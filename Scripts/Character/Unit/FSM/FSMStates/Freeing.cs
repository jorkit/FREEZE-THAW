using Godot;
using System;
using FreezeThaw.Utils;

public partial class Freeing : FSMState
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
        Fsm.character.SelfImage.Play("Freeing");
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Freeing)
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

        /* Freeing the first Sealed Survivor */
        var radiusCheck = Fsm.character.GetNodeOrNull<RadiusCheck>("RadiusCheck");
        if (radiusCheck == null)
        {
            LogTool.DebugLogDump("RadiusCheck not found!");
            return;
        }
        if (radiusCheck.SurvivorsInArea == null)
        {
            LogTool.DebugLogDump("SurvivorsInArea not found!");
            return;
        }
        foreach (var survivor in radiusCheck.SurvivorsInArea)
        {
            if (survivor.GetCurrentState() == CharacterStateEnum.Sealed)
            {
                survivor.Fsm.PreStateChange(CharacterStateEnum.Unsealing, false);
                return;
            }
        }
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
        if (Fsm.PreState == CharacterStateEnum.Freeing)
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