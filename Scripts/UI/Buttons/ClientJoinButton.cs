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
        BigBro.IsMultiplayer = true;
        BigBro.Peer = new();
        BigBro.Spawner = new();
        BigBro.MultiplayerApi = Multiplayer;

        var CreateClientResult = BigBro.Peer.CreateClient("192.168.1.68", 7788);
        if (CreateClientResult != Error.Ok)
        {
            LogTool.DebugLogDump("Client create failed!");
            return;
        }
        BigBro.MultiplayerApi.MultiplayerPeer = BigBro.Peer;
        GetTree().Root.SetMultiplayerAuthority(BigBro.Peer.GetUniqueId());

        //OS.DelayMsec(100);
        Multiplayer.Poll();
        var GetConnectionStatus = BigBro.Peer.GetConnectionStatus();
        if (GetConnectionStatus == MultiplayerPeer.ConnectionStatus.Disconnected)
        {
            LogTool.DebugLogDump("Client connect failed!");
            return;
        }
        CanBePressed = false;

        /* Spawner add */
        BigBro.bigBro.AddChild(BigBro.Spawner);
        BigBro.CreatePlayerContainer();
        foreach (var path in BigBro.CharacterPathList)
        {
            BigBro.Spawner.AddSpawnableScene(path.Value);
        }
        /* PlayerContainer create */
        BigBro.CreatePlayerContainer();
        
        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }
}
