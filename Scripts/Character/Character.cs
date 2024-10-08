using FreezeThaw.Utils;
using Godot;
using System;
using System.Threading;

public abstract partial class Character : CharacterBody2D
{
    public enum CharacterTypeEnum
    {
        /* Monster */
        Sandworm,
        AISandworm,

        /* Survivor */
        Mouse,
        AIMouse
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
        if (BigBro.IsMultiplayer == true)
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
        }
        else
        {
            GlobalPosition = new Vector2(new Random().Next(1000), new Random().Next(1000));
            var MultiplayerSynchronizer = GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer");
            if (MultiplayerSynchronizer != null)
                RemoveChild(MultiplayerSynchronizer);
            if (this != BigBro.Player)
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
        if (Fsm == null)
        {
            LogTool.DebugLogDump("Character not found");
            return;
        }
        if (BigBro.IsMultiplayer != true)
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
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == true)
        {
            SetNewPostion();
        }
    }

    private void SetNewPostion()
    {
        var rpcRes = Rpc("SetNewPostionRpc", Position);
        if (rpcRes != Error.Ok)
        {
            LogTool.DebugLogDump("SetNewPostionRpc failed! " + rpcRes.ToString());
        }
    }

    /* AnyPeer means Calls can from any Peer, no matter if they are node's authority or not */
    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    private void SetNewPostionRpc(Vector2 newPostion)
    {
        var velocity = newPostion - Position;

        /* if far from the NewPostition, reset postion */
        if (velocity.Abs() > new Vector2((float)30, (float)30))
        {
            Position = newPostion;
            return;
        }

        /* debouncing */
        if (velocity.Abs() > new Vector2((float)6, (float)6))
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
        if (BigBro.IsMultiplayer == true)
        {
            /* client do NOT receive the joystick info */
            if (IsMultiplayerAuthority() == false && BigBro.MultiplayerApi.IsServer() == false)
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
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == true)
            {
                if (SelfImage.IsConnected("animation_finished", new Callable(fsmState, "AnimationFinishedHandle")) == false)
                {
                    var connectRes = SelfImage.Connect("animation_finished", new Callable(fsmState, "AnimationFinishedHandle"));
                    if (connectRes != Error.Ok)
                    {
                        LogTool.DebugLogDump("AnimationFinishedHandle connect to singal failed! " + connectRes.ToString());
                    }
                }
            }
        }
        else
        {
            if (SelfImage.IsConnected("animation_finished", new Callable(fsmState, "AnimationFinishedHandle")) == false)
            {
                var connectRes = SelfImage.Connect("animation_finished", new Callable(fsmState, "AnimationFinishedHandle"));
                if (connectRes != Error.Ok)
                {
                    LogTool.DebugLogDump("AnimationFinishedHandle connect to singal failed! " + connectRes.ToString());
                }
            }
        }
    }
}
