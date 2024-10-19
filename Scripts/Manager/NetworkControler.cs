using FreezeThaw.Utils;
using Godot;
using System;

public partial class NetworkControler : Node
{
    /* Multiplayer */
    public static bool IsMultiplayer { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static string Id { set; get; }
    public static bool Connected { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }
    public static bool ReadyStatus { set; get; }

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
        Id = Peer.GetUniqueId().ToString();

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
                PlayerControler.PlayerAdd(id.ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse], false);
                return;
            }
        }
        LogTool.DebugLogDump("[" + id + "]Don't connect");
    }

    public static void PeerDisConnectHandle(long id)
    {
        LogTool.DebugLogDump("Client[" + id + "]Disconnected!");
        PlayerControler.PlayerTranslateToAI(id.ToString());
    }

    public void MultiplayerClientInit(TextEdit serverAddress)
    {
        Peer = new();
        /* Server event handle bind */
        MultiplayerApi = Multiplayer;
        if (MultiplayerApi.IsConnected("connected_to_server", new Callable(this, "ConnectedToServerHandler")) == false)
        {
            MultiplayerApi.Connect("connected_to_server", new Callable(this, "ConnectedToServerHandler"));
            MultiplayerApi.Connect("connection_failed", new Callable(this, "ConnectedFailedHandler"));
            MultiplayerApi.Connect("server_disconnected", new Callable(this, "ServerDisconnectedHandler"));
        }
        /* Create Client and Try to connect */
        var CreateClientResult = Peer.CreateClient(serverAddress.Text, 7788);
        if (CreateClientResult != Error.Ok)
        {
            LogTool.DebugLogDump("Client create failed!");
            serverAddress.Text = "Please check server IP or Domain";
            return;
        }
        /* Peer must be added to MultiplayerPeer (must in connecting state), or the status will never be Connected */
        MultiplayerApi.MultiplayerPeer = Peer;
        Id = Peer.GetUniqueId().ToString();
    }

    private void ServerDisconnectedHandler()
    {
        LogTool.DebugLogDump("Server connect lost!");
        PlayerContainer.TimerStop();
        Peer.Close();
    }

    private void ConnectedToServerHandler()
    {
        Connected = true;
        NetworkControler.IsMultiplayer = true;
        /* set the client sceneTree auth */
        GetTree().Root.SetMultiplayerAuthority(NetworkControler.Peer.GetUniqueId(), true);
        /* Set the RPC target server */
        NetworkControler.Peer.SetTargetPeer((int)MultiplayerPeer.TargetPeerServer);
        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }

    private void ConnectedFailedHandler()
    {
        LogTool.DebugLogDump("Client connect failed!");
        ClientJoinButton.ClientJoinFaild();
        Peer.Close();
    }
}
