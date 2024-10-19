using FreezeThaw.Utils;
using Godot;
using System;

public partial class PreparedArea : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Timer _timer = new();
        _timer.Timeout += PreparedUpdate;
		AddChild(_timer);
		_timer.Start(1);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PreparedUpdate()
	{
		var players = GetNodeOrNull<Node2D>("Players").GetChildren();
		if (players == null)
		{
			LogTool.DebugLogDump("Players not found!");
			return;
		}
		for (int i = 0; i < players.Count; i++)
		{
			((Sprite2D)players[i]).Texture = ResourceLoader.Load(Character.CharacterImagePathList[PlayerContainer.Players[i].SurvivorType]) as Texture2D;
		}
	}
}
