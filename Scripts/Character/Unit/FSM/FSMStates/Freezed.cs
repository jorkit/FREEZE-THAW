using Godot;
using System;
using FreezeThaw.Utils;

public partial class Freezed : FSMState
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
        Fsm.character.SelfImage.Play("Freezed");
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Freezed)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override async void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter!");
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        var FTB = Fsm.character.GetNodeOrNull<FreezeThawButton>("UIContainer/FreezeThawButton");
        if (FTB == null)
        {
            LogTool.DebugLogDump("FTB not found!");
            return;
        }
        FTB.CanBePressed = true;
    }

    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Freezed)
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