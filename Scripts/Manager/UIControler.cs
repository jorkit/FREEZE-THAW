using FreezeThaw.Utils;
using Godot;
using System;

public partial class UIControler : Node
{
    public static Vector2I ScreenSize { set; get; }
    public static Vector2I WindowSize { set; get; }

    public override void _EnterTree()
    {
        ScreenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        LogTool.DebugLogDump(osName);
        if (osName == "Windows")
        {
            WindowSize = new Vector2I(1920, 1080);
            DisplayServer.WindowSetSize(WindowSize);
            /* window at the center of screen */
            var xStart = (ScreenSize.X - DisplayServer.WindowGetSize().X) / 2;
            var yStart = (ScreenSize.Y - DisplayServer.WindowGetSize().Y) / 2;
            DisplayServer.WindowSetPosition(new Vector2I(xStart, yStart));
        }
        else if (osName == "Android")
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            WindowSize = DisplayServer.WindowGetSize();
            LogTool.DebugLogDump(WindowSize.ToString());
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
