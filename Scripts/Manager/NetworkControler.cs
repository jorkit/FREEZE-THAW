using FreezeThaw.Utils;
using Godot;
using System;

public partial class NetworkControler : Node
{
    /* Multiplayer */
    public static bool IsMultiplayer { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        IsMultiplayer = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static bool MultiplayerServerInit()
    {
        IsMultiplayer = true;
        Peer = new();
        Spawner = new();

        var CreateServerResult = Peer.CreateServer(7788);
        if (CreateServerResult != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return false;
        }
        MultiplayerApi.MultiplayerPeer = Peer;

        /* Client event handler bind */
        MultiplayerApi.PeerConnected += new MultiplayerApi.PeerConnectedEventHandler(PeerConnectHandle);
        MultiplayerApi.PeerDisconnected += new MultiplayerApi.PeerDisconnectedEventHandler(PeerDisConnectHandle);

        return true;
    }

    public static void PeerConnectHandle(long id)
    {
        var connectedClients = MultiplayerApi.GetPeers();
        foreach (var connectedClient in connectedClients)
        {
            if (connectedClient == id)
            {
                if (PlayerControler.PlayerContainer.GetNodeOrNull<Character>(id.ToString()) != null)
                {
                    LogTool.DebugLogDump("[" + id + "]Has connected");
                    return;
                }
                LogTool.DebugLogDump("Client[" + id + "]Connected!");
                PlayerControler.PlayerAdd(id.ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse]);
                return;
            }
        }
        LogTool.DebugLogDump("[" + id + "]Don't connect");
    }

    public static void PeerDisConnectHandle(long id)
    {
        LogTool.DebugLogDump("Client[" + id + "]Disconnected!");
        PlayerControler.PlayerRemove(id.ToString());
    }
}
