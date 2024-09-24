using Godot;
using System;
using FreezeThaw.Utils;

public partial class SandwormArmor : FSMState
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
        /* During Armor, Monster can't be Changed state until Die */
        if (true)
        {
            LogTool.DebugLogDump(Name + " Attack play");
            FSM.PreStateChange(Fsm, CharacterStateEnum.Armor, true);
            //return;
        }
        FSM.PreStateChange(Fsm, CharacterStateEnum.Idle, true);
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Armor)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition!");

        return true;
    }
    public override void OnEnter()
    {

        LogTool.DebugLogDump(Name + " Attack OnEnter!");
    }
    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Armor)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " Attack ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " Attack OnExit!");
    }
}
