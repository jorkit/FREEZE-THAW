using Godot;
using System;
using FreezeThaw.Utils;
public partial class SandwormAttack : FSMState
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
        return false;
        LogTool.DebugLogDump("Attack EnterCondition!");
    }
    public override void OnEnter()
	{
		LogTool.DebugLogDump("Attack OnEnter!");
	}
    public override bool ExitCondition()
    {
        LogTool.DebugLogDump("Attack ExitCondition!");
        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump("Attack OnExit!");
    }
}
