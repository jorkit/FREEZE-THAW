using FreezeThaw.Utils;
using Godot;

public partial class ClientJoinButton : TouchScreenButton
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
            LogTool.DebugLogDump("Can't Be Pressed");
            return;
        }
        LogTool.DebugLogDump(Name + " pressed!");
        var ServerAddress = _optionContainer.GetNode<TextEdit>("ServerAddress");
        if (System.String.IsNullOrWhiteSpace(ServerAddress.Text) == true)
        {
            ServerAddress.Text = "Please input server IP or Domain";
            return;
        }
        BigBro.Peer = new();
        /* Server event handle bind */
        BigBro.MultiplayerApi = Multiplayer;
        if (BigBro.MultiplayerApi.IsConnected("connected_to_server", new Callable(this, "ConnectedToServerHandle")) == false)
        {
            BigBro.MultiplayerApi.Connect("connected_to_server", new Callable(this, "ConnectedToServerHandle"));
            BigBro.MultiplayerApi.Connect("connection_failed", new Callable(this, "ConnectedFailedHandle"));
            BigBro.MultiplayerApi.Connect("server_disconnected", new Callable(this, "ServerDisconnectedHandle"));
        }
        /* Create Client and Try to connect */
        var CreateClientResult = BigBro.Peer.CreateClient(ServerAddress.Text, 7788);
        if (CreateClientResult != Error.Ok)
        {
            LogTool.DebugLogDump("Client create failed!");
            ServerAddress.Text = "Please check server IP or Domain";
            return;
        }
        /* Peer must be added to MultiplayerPeer (must in connecting state), or the status will never be Connected */
        BigBro.MultiplayerApi.MultiplayerPeer = BigBro.Peer;
    }

    private void ServerDisconnectedHandle()
    {

    }
    private void ConnectedToServerHandle()
    {
        /* Client connection successful */
        CanBePressed = false;
        BigBro.IsMultiplayer = true;
        /* set the client sceneTree auth */
        GetTree().Root.SetMultiplayerAuthority(BigBro.Peer.GetUniqueId(), true);
        /* Set the RPC target server */
        BigBro.Peer.SetTargetPeer((int)MultiplayerPeer.TargetPeerServer);
        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }

    private void ConnectedFailedHandle()
    {
        LogTool.DebugLogDump("Client connect failed!");
        _optionContainer.GetNode<TextEdit>("ServerAddress").Text = "Please check server IP or Domain";
        BigBro.Peer.Close();
    }
}
