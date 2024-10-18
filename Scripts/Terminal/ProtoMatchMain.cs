using FreezeThaw.Utils;
using Godot;
using System;

public partial class ProtoMatchMain : Node
{
    public override void _EnterTree()
    {
        var playerContainer = PlayerControler.PlayerContainer;
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var _timer = new Timer();
        _timer.Timeout += TimerTimeOutHandler;
        BigBro.bigBro.AddChild(_timer);
        _timer.Start(300);
        //PlayerControler.Players[2].SetScript(ResourceLoader.Load<Script>("res://Scripts/Character/Survivors/AIMouse.cs"));
        //LogTool.DebugLogDump(PlayerControler.Players[2].GetType().ToString());
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void TimerTimeOutHandler()
    {
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.MatchSettlement, true);
    }
}
