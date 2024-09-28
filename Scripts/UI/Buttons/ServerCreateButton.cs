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

        var CreateServerResult = BigBro.Peer.CreateServer(7788);
        if (CreateServerResult != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return;
        }
        CanBePressed = false;
        BigBro.MultiplayerApi.MultiplayerPeer = BigBro.Peer;

        /* Spawner add */
        BigBro.bigBro.AddChild(BigBro.Spawner);
        BigBro.CreatePlayerContainer();
        foreach (var path in BigBro.CharacterPathList)
        {
            BigBro.Spawner.AddSpawnableScene(path.Value);
        }
        /* Client event handler bind */
        BigBro.MultiplayerApi.PeerConnected += new MultiplayerApi.PeerConnectedEventHandler(PeerConnectHandle);
        BigBro.MultiplayerApi.PeerDisconnected += new MultiplayerApi.PeerDisconnectedEventHandler(PeerDisConnectHandle);

        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }

    private void PlayerAdd(long id, NodePath path)
    {
        var character = ResourceLoader.Load<PackedScene>(path).Instantiate();
        character.Name = id.ToString();
        BigBro.PlayerContainer.AddChild(character);
    }
    private void PlayerRemove(long id)
    {
        var quittedClient = BigBro.PlayerContainer.GetNodeOrNull(id.ToString());
        if (quittedClient != null)
        {
            quittedClient.QueueFree();
        }
    }

    public void PeerConnectHandle(long id)
    {
        var connectedClients = BigBro.MultiplayerApi.GetPeers();
        foreach (var connectedClient in connectedClients)
        {
            if (connectedClient == id)
            {
                if (BigBro.PlayerContainer.GetNodeOrNull<Character>(id.ToString()) != null)
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
