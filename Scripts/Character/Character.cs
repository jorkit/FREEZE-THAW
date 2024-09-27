using FreezeThaw.Utils;
using Godot;
using System;
using System.Threading;

public abstract partial class Character : CharacterBody2D
{
    public float Speed;
    private FSM _fsm;
    // public const float JumpVelocity = -400.0f;


    public override void _EnterTree()
    {
        base._EnterTree();
        if (BigBro.IsMultiplayer == true)
        {
            SetMultiplayerAuthority(Name.ToString().ToInt());
            Position = new Vector2(300, 300);
            if (IsMultiplayerAuthority() == false)
            {
                LogTool.DebugLogDump(GetMultiplayerAuthority().ToString());
                RemoveChild(GetNode<UIContainer>("UIContainer"));
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
            }
        }
        else
        {
            RemoveChild(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer"));
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
        var velocity = _fsm.character.GetDirection();
        if (velocity != new Vector2(0, 0))
        {
            _fsm.character.Velocity = velocity.Normalized() * (float)delta * _fsm.character.Speed;
            /* get Collide info */
            var collision_info = _fsm.character.MoveAndCollide(_fsm.character.Velocity);
            if (collision_info != null)
            {
                var collider = collision_info.GetCollider();
                LogTool.DebugLogDump("COlliding!" + collider.GetType().Name);
            }
        }
    }

    public abstract Vector2 GetDirection();
    public CharacterStateEnum GetCurrentState()
    {
        return _fsm.CurrentState.StateIndex;
    }

    public void AttackButtonPressedHandle()
    {
        _fsm.PreStateChange(CharacterStateEnum.Attack, false);
    }
}
