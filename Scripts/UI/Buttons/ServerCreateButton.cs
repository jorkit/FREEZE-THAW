using FreezeThaw.Utils;
using Godot;
using System;
using System.IO;

public partial class ServerCreateButton : TouchScreenButton
{
    private bool CanBePressed;
    private OptionContainer _optionContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _optionContainer = GetParentOrNull<OptionContainer>();
        if (_optionContainer == null)
        {
            LogTool.DebugLogDump("OptionContainer not found!");
            return;
        }
        Pressed += PressedHandler;
        CanBePressed = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressedHandler()
    {
        LogTool.DebugLogDump(Name + " pressed!");
        BigBro.IsMultiplayer = true;
        BigBro.Peer = new();
        BigBro.Spawner = new();

        if (BigBro.Peer.CreateServer(7788) != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return;
        }
        Multiplayer.MultiplayerPeer = BigBro.Peer;
        GetTree().Root.GetNodeOrNull<BigBro>("BigBro").AddChild(BigBro.Spawner);
        Multiplayer.PeerConnected += new MultiplayerApi.PeerConnectedEventHandler(PeerConnectHandle);
        //SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.MatchStartLoading, true);
        var players = ResourceLoader.Load<PackedScene>("res://Scenes/Terminal/MatchMain/ProtoMatchMain/Combo/Players/Players.tscn").InstantiateOrNull<Node>();
        GetParent().GetParent().AddChild(players);
        BigBro.Spawner.SpawnPath = (GetParent().GetParent().GetNode("Players")).GetPath();
        BigBro.Spawner.AddSpawnableScene("res://Scenes/Character/Monsters/Sandworm/Sandworm.tscn");
        PlayerAdd(Multiplayer.GetUniqueId());
    }

    private void PlayerAdd(long id)
    {
        var monster = ResourceLoader.Load<PackedScene>("res://Scenes/Character/Monsters/Sandworm/Sandworm.tscn").InstantiateOrNull<Sandworm>();
        monster.Name = id.ToString();
        GetParent().GetParent().GetNode("Players").AddChild(monster);
    }

    public void PeerConnectHandle(long id)
    {
        LogTool.DebugLogDump("Client[" + id + "]connected!");
        PlayerAdd((long)id);
    }
}
