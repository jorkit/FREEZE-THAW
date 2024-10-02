using FreezeThaw.Utils;
using Godot;
using System;

public partial class ProtoMatchMain : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (BigBro.IsMultiplayer == true)
        {
            var playerContainer = BigBro.PlayerContainer;
            if (playerContainer == null)
            {
                LogTool.DebugLogDump("PlayerContainer not found!");
                return;
            }
            BigBro.bigBro.MoveChild(BigBro.bigBro.GetNodeOrNull<PlayerContainer>("PlayerContainer"), -1);
        }
        else
        {
            var playerContainer = BigBro.PlayerContainer;
            if (playerContainer == null)
            {
                LogTool.DebugLogDump("PlayerContainer not found!");
                return;
            }
            BigBro.bigBro.MoveChild(BigBro.bigBro.GetNodeOrNull<PlayerContainer>("PlayerContainer"), -1);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
