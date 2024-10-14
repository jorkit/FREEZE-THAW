using FreezeThaw.Utils;
using Godot;
using System;

public partial class AudioControler : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Attack(Node2D node, string audioName)
	{
		var audio = GetNodeOrNull<AudioStreamPlayer2D>("Attack/" + audioName);
		if (audio == null)
		{
			LogTool.DebugLogDump("audio not found!");
			return;
		}
        audio.Position = node.Position;
        audio.Play();
	}

	public void Hit(Node2D node, string audioName)
	{
        var audio = GetNodeOrNull<AudioStreamPlayer2D>("Hit/" + audioName);
        if (audio == null)
        {
            LogTool.DebugLogDump("audio not found!");
            return;
        }
		audio.Position = node.Position;
        audio.Play();
    }
}
