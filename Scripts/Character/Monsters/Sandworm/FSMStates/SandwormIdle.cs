using Godot;
using System;
using FreezeThaw.Utils;

public partial class SandwormIdle : FSMState
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
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState > CharacterStateEnum.Run || Joystick.GetCurPosition() != new Vector2(0, 0))
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");
        Fsm.PreStateChange(CharacterStateEnum.Run, false);

        return true;
    }
    public override void OnEnter()
	{
        LogTool.DebugLogDump(Name + " OnEnter!");
	}
    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Idle)
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
