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
        Fsm.character.SelfImage.Play("Freezing");
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
        Fsm.character.AnimatitionFinishedHandleRegiste(this);
    }

    private void AnimationFinishedHandle()
    {
        Fsm.PreStateChange(CharacterStateEnum.Freezed, true);
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