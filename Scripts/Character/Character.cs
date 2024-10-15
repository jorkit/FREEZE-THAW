using FreezeThaw.Utils;
using Godot;
using System;
using System.ComponentModel;
using System.Threading;

public abstract partial class Character : CharacterBody2D
{
    public enum CharacterTypeEnum
    {
        /* AI */
        AISandworm = -2,
        AIMouse,

        /* Monster */
        Sandworm,
        /* Survivor */
        Mouse
    }
    public static readonly Godot.Collections.Dictionary<CharacterTypeEnum, string> CharacterPathList = new Godot.Collections.Dictionary<CharacterTypeEnum, string>()
    {
        [CharacterTypeEnum.Sandworm] = "res://Scenes/Character/Monsters/Sandworm.tscn",
        [CharacterTypeEnum.AISandworm] = "res://Scenes/Character/Monsters/AISandworm.tscn",
        [CharacterTypeEnum.Mouse] = "res://Scenes/Character/Survivors/Mouse.tscn",
        [CharacterTypeEnum.AIMouse] = "res://Scenes/Character/Survivors/AIMouse.tscn"
    };

    public FSM Fsm;
    public float Speed;
    public AnimatedSprite2D SelfImage { get; set; }
    // public const float JumpVelocity = -400.0f;

    public override void _EnterTree()
    {
        if (NetworkControler.IsMultiplayer == true)
        {
            /* Set the authority of this node */
            SetMultiplayerAuthority(Name.ToString().ToInt(), true);

            if (Position == Vector2.Zero)
            {
                Position = new Vector2(new Random().Next(1000), new Random().Next(1000));
            }
            if (IsMultiplayerAuthority() == false)
            {
                /* hide the other clients' UIContainer and remove their Camera */
                GetNode<UIContainer>("UIContainer").Visible = false;
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
                return;
            }
            /* AI in multiplayer */
            if (GetType().BaseType.BaseType != typeof(Character))
            {
                /* hide the AIs' UIContainer and remove their Camera */
                GetNode<UIContainer>("UIContainer").Visible = false;
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
                return;
            }
        }
        else
        {
            if (Position == Vector2.Zero)
            {
                Position = new Vector2(new Random().Next(1000), new Random().Next(1000));
            }
            if (this != PlayerControler.Player)
            {
                GetNode<UIContainer>("UIContainer").Visible = false;
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
                return;
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Fsm = GetNodeOrNull<FSM>("FSM");
        if (Fsm == null)
        {
            LogTool.DebugLogDump("FSM not found!");
            return;
        }
        SelfImage = GetNodeOrNull<AnimatedSprite2D>("SelfImage");
        if (SelfImage == null)
        {
            LogTool.DebugLogDump("SelfImage not found!");
            return;
        }
        GetNodeOrNull<Polygon2D>("AttackDirection").Visible = false;
        /* add scoreLabel */
        var scoreLabel = new Label()
        {
            Name = "ScoreLabel",
            Text = "300",
            Position = new Vector2(-20, -150)
        };
        
        AddChild(scoreLabel);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GetType().BaseType.BaseType != typeof(Character))
        {
            AIRunning();
        }

        if (NetworkControler.IsMultiplayer != true)
        {
            if (GetCurrentState() != CharacterStateEnum.Run && GetCurrentState() != CharacterStateEnum.Hurt)
            {
                return;
            }
        }
        
        /* get Collide info */
        if (Fsm.character.MoveAndSlide() == true)
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                //LogTool.DebugLogDump(Name + " COlliding!" + GetSlideCollision(i).GetType().Name);
                var collider = GetSlideCollision(i).GetCollider();
                if (collider == null)
                {
                    LogTool.DebugLogDump("Collider not found!");
                    continue;
                }
            }
        }
        /* Multiplayer and server do RPC call */
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == true)
        {
            SetNewPosition();
        }
    }

    private void SetNewPosition()
    {
        var rpcRes = Rpc("SetNewPositionRpc", Position);
        if (rpcRes != Error.Ok)
        {
            LogTool.DebugLogDump("SetNewPositionRpc failed! " + rpcRes.ToString());
        }
    }

    /* AnyPeer means Calls can from any Peer, no matter if they are node's authority or not */
    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    private void SetNewPositionRpc(Vector2 newPosition)
    {
        var velocity = newPosition - Position;

        /* if far from the NewPostition, reset postion */
        if (velocity.Abs().X > 60 || velocity.Abs().Y > 60)
        {
            Position = newPosition;
            return;
        }

        /* debouncing */
        if (velocity.Abs().X > 6 || velocity.Abs().Y > 6)
        {
            Velocity = velocity.Normalized() * Speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    public Vector2 GetDirection()
    {
        if (NetworkControler.IsMultiplayer == true)
        {
            /* client do NOT receive the joystick info */
            if (NetworkControler.MultiplayerApi.IsServer() == false)
            {
                return Vector2.Zero;
            }
            return GetNodeOrNull<Joystick>("UIContainer/Joystick").GetCurPosition();
        }
        else
        {
            return GetNodeOrNull<Joystick>("UIContainer/Joystick").GetCurPosition();
        }
    }
    public CharacterStateEnum GetCurrentState()
    {
        return Fsm.CurrentState.StateIndex;
    }

    public void AttackButtonPressedHandle()
    {
        Fsm.PreStateChange(CharacterStateEnum.Attack, false);
    }

    public virtual void Attack()
    {
    }

    public abstract void FreezeThawButtonPressedHandle();

    public void AnimatitionFinishedHandleRegiste(FSMState fsmState)
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() != true)
        {
            return;
        }
        if (SelfImage.IsConnected("animation_finished", new Callable(fsmState, "AnimationFinishedHandler")) == false)
        {
            var connectRes = SelfImage.Connect("animation_finished", new Callable(fsmState, "AnimationFinishedHandler"));
            if (connectRes != Error.Ok)
            {
                LogTool.DebugLogDump("AnimationFinishedHandler connect to singal failed! " + connectRes.ToString());
            }
        }
    }

    /* AI related */
    private void AIRunning()
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == false)
        {
            return;
        }
        if (PlayerControler.Monster == null)
        {
            LogTool.DebugLogDump("Wait for Translating");
            return;
        }
        var point = GetNodeOrNull<Sprite2D>("UIContainer/Joystick/Point");
        if (point == null)
        {
            LogTool.DebugLogDump("Joystick point not found!");
            return;
        }
        /* AI Monster */
        if (GetType().BaseType.BaseType == typeof(Monster))
        {
            AiFTBTrigger();
            /* Detect the survivor */
            if (GetNodeOrNull<AIRadiusCheck>("AIRadiusCheck").SurvivorsInArea.Count <= 0)
            {
                point.Position = Vector2.Zero;
                return;
            }
            var target = GetNodeOrNull<AIRadiusCheck>("AIRadiusCheck").SurvivorsInArea.Find(survivor => survivor.GetCurrentState() != CharacterStateEnum.Sealed);
            if (target == null)
            {
                point.Position = Vector2.Zero;
                return;
            }
            /* chase the survivor */
            if (Position.DistanceTo(target.Position) > 150)
            {
                point.Position = (target.Position - Position).Normalized();
            }
            /* attach the survivor */
            else
            {
                point.Position = Vector2.Zero;
                var attackDirection = (target.Position - Position).Normalized();
                if (attackDirection == Vector2.Zero)
                {
                    return;
                }
                AiATBTrigger(attackDirection);
            }
        }
        /* AI Survivor */
        else
        {
            /* Freezing or Sealed can do Nothing */
            if (GetCurrentState() == CharacterStateEnum.Freezing || GetCurrentState() > CharacterStateEnum.Freezed)
            {
                return;
            }
            /* Freeing other survivor */
            var survivor = GetNodeOrNull<RadiusCheck>("RadiusCheck").SurvivorsInArea.Find(survivor => survivor.GetCurrentState() == CharacterStateEnum.Sealed);
            if (survivor != null)
            {
                AiFTBTrigger();
                return;
            }
            /* Freezed state */
            if (GetCurrentState() == CharacterStateEnum.Freezed)
            {
                AiFTBTrigger();
                return;
            }
            /* Freezing to protect self */
            if (Position.DistanceTo(PlayerControler.Monster.Position) < 150)
            {
                AiFTBTrigger();
                return;
            }
            /* control the distance to Monster large than 1000 */
            else if (Position.DistanceTo(PlayerControler.Monster.Position) < 1000)
            {
                point.Position = (Position - PlayerControler.Monster.Position).Normalized();
            }
            else if (Position.DistanceTo(PlayerControler.Monster.Position) > 1100)
            {
                point.Position = (PlayerControler.Monster.Position - Position).Normalized();
            }
            /* attack in safe distance */
            else
            {
                point.Position = Vector2.Zero;
                var attackDirection = (PlayerControler.Monster.Position - Position).Normalized();
                if (attackDirection == Vector2.Zero)
                {
                    return;
                }
                AiATBTrigger(attackDirection);
            }
        }
    }


    private void AiFTBTrigger()
    {
        var FTB = GetNodeOrNull<FreezeThawButton>("UIContainer/FreezeThawButton");
        if (FTB == null)
        {
            LogTool.DebugLogDump("FTB not found!");
            return;
        }
        if (FTB.CanBePressed == true)
        {
            FTB.PressedHandle();
            return;
        }
    }

    private void AiATBTrigger(Vector2 direction)
    {
        var ATB = GetNodeOrNull<AttackButton>("UIContainer/AttackButton");
        if (ATB == null)
        {
            LogTool.DebugLogDump("ATB not found!");
            return;
        }
        if (ATB.CanBePressed == true)
        {
            ATB.SetNewPosition(direction);
            ATB.ReleaseHandle();
        }
    }
}
