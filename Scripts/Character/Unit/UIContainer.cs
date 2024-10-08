using Godot;
using System;
using FreezeThaw.Utils;

public partial class UIContainer : CanvasLayer
{
    public Character character;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        character = GetParent<Character>();
        if (character == null)
        {
            LogTool.DebugLogDump("Character not found!");
            return;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
