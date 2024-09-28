using FreezeThaw.Utils;
using Godot;
using System;

public partial class WaitingHall : Node
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
        BigBro.Spawner.SpawnPath = GetNodeOrNull<Node>("PlayerContainer").GetPath();
        if (BigBro.MultiplayerApi.IsServer() == true)
        {
            BigBro.PlayerAdd(GetMultiplayerAuthority(), BigBro.CharacterPathList[BigBro.CharacterTypeEnum.Sandworm]);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
