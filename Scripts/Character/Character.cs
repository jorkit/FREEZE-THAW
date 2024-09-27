using FreezeThaw.Utils;
using Godot;
using System;

public abstract partial class Character : CharacterBody2D
{
    public float Speed;
    private FSM _fsm;
    protected PackedScene Bullet;
    // public const float JumpVelocity = -400.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _fsm = GetNodeOrNull<FSM>("FSM");
        if (_fsm == null)
        {
            LogTool.DebugLogDump("FSM not found!");
            return;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        /* if Joystick move, try to set Run state */
        if (Joystick.GetCurPosition() != new Vector2(0, 0))
        {
            FSM.PreStateChange(_fsm, CharacterStateEnum.Run, false);
        }
        /* If a is stationary and CurrentState is less than Run, set Idle state force */
        else if (_fsm.PreState <= CharacterStateEnum.Run)
        {
            FSM.PreStateChange(_fsm, CharacterStateEnum.Idle, true);
        }
    }

    public abstract Vector2 GetDirection();
    public CharacterStateEnum GetCurrentState()
    {
        return _fsm.CurrentState.StateIndex;
    }

    public void AttackButtonPressedHandle()
    {
        FSM.PreStateChange(_fsm, CharacterStateEnum.Attack, false);
    }

    public virtual void Attack()
    {
        var bullet = Bullet.Instantiate<Bullet>();
        bullet.SetDirection(new Vector2(1, 0));
        GetParent().AddChild(bullet);
        bullet.GlobalPosition = GetNode<Marker2D>("BulletBornPosition").GlobalPosition;
    }
}
