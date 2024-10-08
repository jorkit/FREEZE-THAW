using Godot;
using System;
using FreezeThaw.Utils;
public partial class Hurt : FSMState
{
    private int _knockbackSpeed = 5;
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
        Fsm.character.SelfImage.Play("Hurt");
        if (Fsm.character.GetType().BaseType == typeof(Survivor) || Fsm.character.GetType().BaseType.BaseType == typeof(Survivor))
        {
            var hurtDirection = BigBro.Monster.Position.DirectionTo(Fsm.character.Position).Normalized() * _knockbackSpeed;
            Fsm.character.Position += hurtDirection;
        }
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Hurt)
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
        if (Fsm.CurrentState != this)
        {
            return;
        }
        Fsm.PreStateChange(CharacterStateEnum.Idle, true);
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
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == false)
        {
            return;
        }
        PlayerContainer.PlayerTranslate(Fsm.character.Name);
    }
}