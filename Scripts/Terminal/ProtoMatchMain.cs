using FreezeThaw.Utils;
using Godot;
using System;

public partial class ProtoMatchMain : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        var players = BigBro.PlayerContainer;
        if (players == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
        AddChild(players);
        var player = ResourceLoader.Load<PackedScene>(BigBro.CharacterPathList[BigBro.CharacterTypeEnum.Sandworm]).InstantiateOrNull<Sandworm>();
        if (player == null)
        {
            LogTool.DebugLogDump("Character not found!");
            return;
        }
        var playerContainer = GetNodeOrNull<Node>("PlayerContainer");
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
        playerContainer.AddChild(player);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
