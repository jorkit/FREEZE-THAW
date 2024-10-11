using Godot;
using System;
using FreezeThaw.Utils;
using static System.Net.Mime.MediaTypeNames;

public partial class Attack : FSMState
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
        Fsm.character.SelfImage.Play("Attack");
    }

    public override bool EnterCondition()
    {
        if (Fsm.PreState != CharacterStateEnum.Attack)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition!");

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
        if (Fsm.character.GetType().BaseType == typeof(Monster) || Fsm.character.GetType().BaseType.BaseType == typeof(Monster))
        {
            ((Monster)Fsm.character).AttackArea.CollisionMask = 0;
        }
        Fsm.PreStateChange(CharacterStateEnum.Idle, true);
    }

    public override bool ExitCondition()
    {
        if (Fsm.PreState == CharacterStateEnum.Attack)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit!");
        Fsm.character.GetNodeOrNull<AttackButton>("UIContainer/AttackButton").CanBePressed = true;
    }
}
