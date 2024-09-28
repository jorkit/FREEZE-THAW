using Godot;
using System;
using FreezeThaw.Utils;

public partial class Run : FSMState
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
        var velocity = Fsm.character.GetDirection();
        if (velocity != Vector2.Zero)
        {
            Fsm.character.Velocity = velocity.Normalized() * Fsm.character.Speed;
            Fsm.character.SelfImage.Play("Run");
        }
    }

    public override bool EnterCondition()
    {
        /* if in Idle and Joystick move, try to set Run Prestate */
        if (Fsm.PreState > CharacterStateEnum.Run || Fsm.character.GetDirection() == Vector2.Zero)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");
        Fsm.PreStateChange(CharacterStateEnum.Run, false);

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter");
    }
    public override bool ExitCondition()
    {
        if (Fsm.PreState <= CharacterStateEnum.Run && Fsm.character.GetDirection() != Vector2.Zero)
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