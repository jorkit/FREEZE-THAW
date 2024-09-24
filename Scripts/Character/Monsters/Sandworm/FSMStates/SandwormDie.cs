using Godot;
using System;
using FreezeThaw.Utils;
public partial class SandwormDie : FSMState
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
        LogTool.DebugLogDump("Armor EnterCondition!");
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump("Armor OnEnter!");
    }
    public override bool ExitCondition()
    {
        LogTool.DebugLogDump("Armor ExitCondition!");
        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump("Armor OnExit!");
    }
}
