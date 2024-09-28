using FreezeThaw.Utils;
using Godot;
using Godot.Collections;
using System;

//GetNode<Monster>("/root/Main/Monster").SetScript(ResourceLoader.Load("res://Scripts/Characters/Monsters/AI.cs"));
//LogTool.DebugLogDump("set script");
public partial class BigBro : Node
{
    public static BigBro bigBro { set; get; }
    public static Vector2I screenSize {  set; get; }
    public static Vector2I windowSize { set; get; }

    public enum CharacterTypeEnum
    {
        /* Monster */
        Sandworm,

        /* Survivor */
        Mouse
    }

    public static bool IsMultiplayer { set; get; }
    public static Node PlayerContainer { set; get; }
    public static readonly string PlayerContainerPath = "res://Scenes/Manager/PlayerContainer.tscn";
    public static readonly Dictionary<CharacterTypeEnum, string> CharacterPathList = new Dictionary<CharacterTypeEnum, string>()
    {
        [CharacterTypeEnum.Sandworm] = "res://Scenes/Character/Monsters/Sandworm.tscn",
        [CharacterTypeEnum.Mouse] = "res://Scenes/Character/Survivors/Mouse.tscn",
    };
    public static Monster Monster { set; get; }
    public static Godot.Collections.Array<Survivor> Survivors { set; get; }
    public static Character Player { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }

    public static SceneFSM SceneFSM { set; get; }

    public override void _EnterTree()
    {
        base._EnterTree();
        IsMultiplayer = false;
        screenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        LogTool.DebugLogDump(osName);
        if (osName == "Windows")
        {
            windowSize = new Vector2I(1120, 780);
            DisplayServer.WindowSetSize(windowSize);
            /* window at the center of screen */
            var xStart = (screenSize.X - DisplayServer.WindowGetSize().X) / 2;
            var yStart = (screenSize.Y - DisplayServer.WindowGetSize().Y) / 2;
            DisplayServer.WindowSetPosition(new Vector2I(xStart, yStart));
        }
        else if (osName == "Android")
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            windowSize = DisplayServer.WindowGetSize();
            LogTool.DebugLogDump(windowSize.ToString());
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BigBro.bigBro = this;
        SceneFSM = GetNodeOrNull<SceneFSM>("SceneFSM");
        SceneFSM.SetInitState();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public static void CreatePlayerContainer()
    {
        var playerContainer = ResourceLoader.Load<PackedScene>(BigBro.PlayerContainerPath).InstantiateOrNull<Node>();
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
        BigBro.PlayerContainer = playerContainer;
    }
}
