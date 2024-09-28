using Godot;
using System;
using FreezeThaw.Utils;

public partial class Idle : FSMState
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
        if (ExitCondition() == false)
        {
            Fsm.character.SelfImage.Play("Idle");
            //LogTool.DebugLogDump(Name + " Animation play");
            //return;
        }
    }

    public override bool EnterCondition()
    {
        /* if PreState is large than Run or joystick move, return*/
        if (Fsm.PreState > CharacterStateEnum.Run || Fsm.character.GetDirection() != Vector2.Zero)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter!");
    }
    public override bool ExitCondition()
    {
        /* if PreState is less or equla to Run and joystick do not move, return*/
        if (Fsm.PreState <= CharacterStateEnum.Run && Fsm.character.GetDirection() == Vector2.Zero)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit!");
    }
}