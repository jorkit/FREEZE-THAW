using Godot;
using System;
using FreezeThaw.Utils;

public partial class Unsealing : FSMState
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
        Fsm.character.SelfImage.Play("Unsealing");
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Unsealing)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }

    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter!");
        Fsm.character.AnimatitionFinishedHandleRegiste(this);
        Fsm.character.Attack();
    }

    private void AnimationFinishedHandler()
    {
        if (Fsm.CurrentState != this)
        {
            return;
        }
        Fsm.PreStateChange(CharacterStateEnum.Idle, true);
    }

    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Unsealing)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit!");
        var FTB = Fsm.character.GetNodeOrNull<FreezeThawButton>("UIContainer/FreezeThawButton");
        if (FTB == null)
        {
            LogTool.DebugLogDump("FTB not found!");
            return;
        }
        FTB.CanBePressed = true;
    }
}