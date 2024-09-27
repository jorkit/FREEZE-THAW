﻿using Godot;
using System;
using FreezeThaw.Utils;
public partial class SandwormRun : FSMState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Update(double delta)
    {
<<<<<<< HEAD
        var velocity = Fsm.character.GetDirection();
        if (velocity == new Vector2(0, 0))
        {
            var left = Input.IsActionPressed("ui_left");
            var right = Input.IsActionPressed("ui_right");
            var up = Input.IsActionPressed("ui_up");
            var down = Input.IsActionPressed("ui_down");

            if (left)
            {
                velocity.X--;
            }
            if (right)
            {
                velocity.X++;
            }
            if (up)
            {
                velocity.Y--;
            }
            if (down)
            {
                velocity.Y++;
            }
        }
        Fsm.character.Velocity = velocity.Normalized() * (float)delta * Fsm.character.Speed;

        /* get Collide info */
        var collision_info = Fsm.character.MoveAndCollide(Fsm.character.Velocity);
        if (collision_info != null)
        {
            var collider = collision_info.GetCollider();
            LogTool.DebugLogDump("COlliding!" + collider.GetType().Name);
        }
=======
>>>>>>> b10c988 (1. joystick adapt to xbox)
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Run)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
	{
        LogTool.DebugLogDump(Name + " OnEnter");
    }
    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Run)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
    }
}
