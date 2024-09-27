
using Godot;
using System;
using FreezeThaw.Utils;
public partial class Die : FSMState
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
            LogTool.DebugLogDump(Name + " Dying play");
            return;
        }
        LogTool.DebugLogDump(Name + " Dead play");
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Die)
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
        /* Die is the last state */
        return false;
    }
    public override void OnExit()
    {
        return;
    }
}
