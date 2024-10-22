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
			if (PlayerContainer.Players.Count < i + 1 )
			{
                ((TextureButton)players[i]).TextureNormal = ResourceLoader.Load("res://Static/UI/Joystick/摇杆.webp") as Texture2D;
                return;
			}
			((TextureButton)players[i]).TextureNormal = ResourceLoader.Load(Character.CharacterImagePathList[PlayerContainer.Players[i].SurvivorType]) as Texture2D;
			((TextureButton)players[i]).Scale = new Vector2((float)0.3, (float)0.3);
        }
        Position = new Vector2(UIControler.WindowSize.X * 3 / 4, UIControler.WindowSize.Y / 2);
    }
}
