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
        var CreateClientResult = BigBro.Peer.CreateClient(ServerAddress.Text, 7788);
        if (CreateClientResult != Error.Ok)
        {
            LogTool.DebugLogDump("Client create failed!");
            ServerAddress.Text = "Please check server IP or Domain";
            return;
        }
        /* Peer must be added to MultiplayerPeer, or the status will never be Connected */
        BigBro.MultiplayerApi = Multiplayer;
        BigBro.MultiplayerApi.MultiplayerPeer = BigBro.Peer;

        /* must poll, or the status will never be Connected */
        int i = 10;
        do
        {
            Multiplayer.Poll();
            var GetConnectionStatus = BigBro.Peer.GetConnectionStatus();
            if (GetConnectionStatus == MultiplayerPeer.ConnectionStatus.Connected)
            {
                i = -1;
                break;
            }
            OS.DelayMsec(100);
        } while (--i > 0);
        if (i != -1)
        {
            LogTool.DebugLogDump("Client connect failed!");
            ServerAddress.Text = "Please check server IP or Domain";
            BigBro.Peer.Close();
            return;
        }

        /* Client connection successful */
        CanBePressed = false;
        BigBro.IsMultiplayer = true;
        GetTree().Root.SetMultiplayerAuthority(BigBro.Peer.GetUniqueId());

        /* Spawner add */
        BigBro.Spawner = new();
        BigBro.bigBro.AddChild(BigBro.Spawner);
        BigBro.CreatePlayerContainer();
        foreach (var path in BigBro.CharacterPathList)
        {
            BigBro.Spawner.AddSpawnableScene(path.Value);
        }
        
        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }
}
