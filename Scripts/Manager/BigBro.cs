using FreezeThaw.Utils;
using Godot;
using System;

//GetNode<Monster>("/root/Main/Monster").SetScript(ResourceLoader.Load("res://Scripts/Characters/Monsters/AI.cs"));
//LogTool.DebugLogDump("set script");
public partial class BigBro : Node
{
    public static Vector2I screenSize;
    public static Vector2I windowSize;

    public static Monster monster;
    public static Godot.Collections.Array<Survivor> survivors;
    public static Character Player;

    public static SceneFSM SceneFSM { set; get; }

    public override void _EnterTree()
    {
        base._EnterTree();
        screenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        LogTool.DebugLogDump(osName);
        if (osName == "Windows")
        {
            windowSize = new Vector2I(1120, 680);
            DisplayServer.WindowSetSize(windowSize);
            LogTool.DebugLogDump(windowSize.ToString());
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
