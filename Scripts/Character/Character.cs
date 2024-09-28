using FreezeThaw.Utils;
using Godot;
using System;
using System.Threading;

public abstract partial class Character : CharacterBody2D
{
    private FSM _fsm;
    public float Speed;
    public AnimatedSprite2D SelfImage {  get; set; }
    // public const float JumpVelocity = -400.0f;

    public override void _EnterTree()
    {
        base._EnterTree();
        if (BigBro.IsMultiplayer == true)
        {
            /* Set the authority of this node */
            SetMultiplayerAuthority(Name.ToString().ToInt());
            Position = new Vector2(300, 300);
            if (IsMultiplayerAuthority() == false)
            {
                RemoveChild(GetNode<UIContainer>("UIContainer"));
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
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        /* if not self, return */
        if (IsMultiplayerAuthority() == false)
        {
            return;
        }
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
        _fsm.character.Velocity = Vector2.Zero;
    }

    public Vector2 GetDirection()
    {
        if (IsMultiplayerAuthority() == false)
        {
            return Vector2.Zero;
        }
        return Joystick.GetCurPosition();
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
}
