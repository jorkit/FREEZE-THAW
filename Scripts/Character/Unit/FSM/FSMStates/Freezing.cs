using Godot;
using System;
using FreezeThaw.Utils;

public partial class Freezing : FSMState
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
        if (true)
        {
            LogTool.DebugLogDump(Name + " Freezing play");
            Fsm.PreStateChange(CharacterStateEnum.Freezing, true);
            //return;
        }
        Fsm.PreStateChange(CharacterStateEnum.Idle, true);
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Freezing)
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
        if (Fsm.PreState == CharacterStateEnum.Freezing)
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