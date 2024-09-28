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
        if (CanBePressed == false)
        {
            return;
        }
        LogTool.DebugLogDump(Name + " pressed!");
        BigBro.IsMultiplayer = true;
        BigBro.Peer = new();
        BigBro.Spawner = new();
        BigBro.MultiplayerApi = Multiplayer;

        if (BigBro.Peer.CreateServer(7788) != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return;
        }
        Multiplayer.MultiplayerPeer = BigBro.Peer;
        GetTree().Root.GetNodeOrNull<BigBro>("BigBro").AddChild(BigBro.Spawner);
        //SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.MatchStartLoading, true);
        var players = ResourceLoader.Load<PackedScene>(BigBro.PlayersPath).InstantiateOrNull<Node>();
        GetParent().GetParent().AddChild(players);
        BigBro.Players = GetParent().GetParent().GetNode("Players");
        BigBro.Spawner.SpawnPath = BigBro.Players.GetPath();
        foreach (var path in BigBro.CharacterPathList)
        {
            BigBro.Spawner.AddSpawnableScene(path.Value);
        }

        PlayerAdd(Multiplayer.GetUniqueId(), BigBro.CharacterPathList[BigBro.CharacterTypeEnum.Sandworm]);
        Multiplayer.PeerConnected += new MultiplayerApi.PeerConnectedEventHandler(PeerConnectHandle);
        Multiplayer.PeerDisconnected += new MultiplayerApi.PeerDisconnectedEventHandler(PeerDisConnectHandle);
        CanBePressed = false;
        _optionContainer.Visible = false;
    }

    private void PlayerAdd(long id, NodePath path)
    {
        var character = ResourceLoader.Load<PackedScene>(path).Instantiate();
        character.Name = id.ToString();
        LogTool.DebugLogDump("I'm " + Multiplayer.GetUniqueId() + ", " + character.Name + " add!!!");
        BigBro.Players.AddChild(character);
    }
    private void PlayerRemove(long id)
    {
        LogTool.DebugLogDump("id: " + id.ToString());
        var quittedClient = BigBro.Players.GetNodeOrNull(id.ToString());
        if (quittedClient != null)
        {
            quittedClient.QueueFree();
        }
    }

    public void PeerConnectHandle(long id)
    {
        var connectedClients = Multiplayer.GetPeers();
        foreach (var connectedClient in connectedClients)
        {
            if (connectedClient == id)
            {
                if (BigBro.Players.GetNodeOrNull<Character>(id.ToString()) != null)
                {
                    LogTool.DebugLogDump("[" + id + "]Has connected");
                    return;
                }
                LogTool.DebugLogDump("Client[" + id + "]Connected!");
                PlayerAdd((long)id, BigBro.CharacterPathList[BigBro.CharacterTypeEnum.Mouse]);
                return;
            }
        }
        LogTool.DebugLogDump("[" + id + "]Don't connect");
    }

    public void PeerDisConnectHandle(long id)
    {
        LogTool.DebugLogDump("Client[" + id + "]Disconnected!");
        PlayerRemove((long)id);
    }
}
