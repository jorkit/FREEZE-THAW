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
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == true)
        {
            if (Fsm.character.SelfImage.IsConnected("animation_finished", new Callable(this, "AnimationFinishedHandle")) == false)
            {
                var connectRes = Fsm.character.SelfImage.Connect("animation_finished", new Callable(this, "AnimationFinishedHandle"));
                if (connectRes != Error.Ok )
                {
                    LogTool.DebugLogDump("AnimationFinishedHandle connect to singal failed! " + connectRes.ToString());
                }
            }
        }
        Fsm.character.Attack();
    }

    public void AnimationFinishedHandle()
    {
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
    }
}
