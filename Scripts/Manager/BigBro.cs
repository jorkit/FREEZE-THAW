using FreezeThaw.Utils;
using Godot;
using System;

//GetNode<Monster>("/root/Main/Monster").SetScript(ResourceLoader.Load("res://Scripts/Characters/Monsters/AI.cs"));
//LogTool.DebugLogDump("set script");
public partial class BigBro : Node
{
    public static Vector2I screenSize;
    public static Vector2I windowSize;


    public static bool IsMultiplayer { set; get; }
    public static Node Players { set; get; }
    public static Monster Monster { set; get; }
    public static Godot.Collections.Array<Survivor> Survivors { set; get; }
    public static Character Player { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }
    public static MultiplayerSynchronizer SpawnerSynchronizer { set; get; }

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
        SceneFSM = GetNodeOrNull<SceneFSM>("SceneFSM");
        SceneFSM.SetInitState();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
