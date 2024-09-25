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
        LogTool.DebugLogDump(Name + " pressed!");
        BigBro.IsMultiplayer = true;
        BigBro.Peer = new();
        BigBro.Spawner = new();

        GetTree().Root.GetNodeOrNull<BigBro>("BigBro").AddChild(BigBro.Spawner);
        var players = ResourceLoader.Load<PackedScene>("res://Scenes/Terminal/MatchMain/ProtoMatchMain/Combo/Players/Players.tscn").InstantiateOrNull<Node>();
        GetParent().GetParent().AddChild(players);
        BigBro.Spawner.SpawnPath = (GetParent().GetParent().GetNode("Players")).GetPath();
        BigBro.Spawner.AddSpawnableScene("res://Scenes/Character/Monsters/Sandworm/Sandworm.tscn");

        if (BigBro.Peer.CreateClient("192.168.1.68", 7788) != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return;
        }
        Multiplayer.MultiplayerPeer = BigBro.Peer;
    }
}
