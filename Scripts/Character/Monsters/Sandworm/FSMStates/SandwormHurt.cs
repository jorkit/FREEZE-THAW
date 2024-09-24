using Godot;
using System;
using FreezeThaw.Utils;
public partial class SandwormHurt : FSMState
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
        /* During Hurt, Monster can Changed state */
        if (true)
        {
            LogTool.DebugLogDump(Name + " Hurt play");
            return;
        }
        ((Monster)Fsm.character).Hurting = false;
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Hurt)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        /* Hurt return false forever */
        return false;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter!");
    }
    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Hurt)
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
