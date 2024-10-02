using FreezeThaw.Utils;
using Godot;
using System;
using System.Threading;

public abstract partial class Character : CharacterBody2D
{
    protected FSM _fsm;
    public float Speed;
    public AnimatedSprite2D SelfImage { get; set; }
    // public const float JumpVelocity = -400.0f;

    public override void _EnterTree()
    {
        if (BigBro.IsMultiplayer == true)
        {
            /* Set the authority of this node */
            LogTool.DebugLogDump(GetMultiplayerAuthority().ToString());
            SetMultiplayerAuthority(Name.ToString().ToInt(), true);
            LogTool.DebugLogDump(GetMultiplayerAuthority().ToString());
            

            Position = new Vector2(new Random().Next(1000), new Random().Next(1000));
            if (IsMultiplayerAuthority() == false && BigBro.MultiplayerApi.IsServer() == false)
            {
                /* hide the other clients' UIContainer and remove their Camera */
                GetNode<UIContainer>("UIContainer").Visible = false;
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
            }
        }
        else
        {
            var MultiplayerSynchronizer = GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer");
            if (MultiplayerSynchronizer != null)
                RemoveChild(MultiplayerSynchronizer);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _fsm = GetNodeOrNull<FSM>("FSM");
        if (_fsm == null)
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
        var scoreLabel = new Label();
        scoreLabel.Name = "ScoreLabel";
        scoreLabel.Text = "300";
        scoreLabel.Position = new Vector2(-20, -150);
        AddChild(scoreLabel);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_fsm == null)
        {
            LogTool.DebugLogDump("Character not found");
            return;
        }
        
        /* get Collide info */
        if (_fsm.character.MoveAndSlide() == true)
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                LogTool.DebugLogDump("COlliding!" + GetSlideCollision(i).GetType().Name);
            }
        }
        SetNewPostion();
    }

    private void SetNewPostion()
    {
        /* Multiplayer and server do Ppc call */
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == true)
        {
            var rpcRes = Rpc("SetNewPostionRpc", Position);
            if (rpcRes != Error.Ok)
            {
                LogTool.DebugLogDump("SetNewPostionRpc failed! " + rpcRes.ToString());
            }
        }
    }

    /* AnyPeer means Calls can from any Peer, no matter if they are node's authority or not */
    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    private void SetNewPostionRpc(Vector2 newPostion)
    {
        var velocity = newPostion - Position;
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
        }
        return GetNodeOrNull<Joystick>("UIContainer/Joystick").GetCurPosition();
    }
    public CharacterStateEnum GetCurrentState()
    {
        return _fsm.CurrentState.StateIndex;
    }

    public void AttackButtonPressedHandle()
    {
        _fsm.PreStateChange(CharacterStateEnum.Attack, false);
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
