using FreezeThaw.Utils;
using Godot;
using System;

public partial class UIControler : Node2D
{
    public static Vector2I ScreenSize { set; get; }
    public static Vector2I WindowSize { set; get; }

    public static CanvasLayer SettingContainer { set; get; }

    public static CanvasLayer UserInterfaceContainer { set; get; }

    public override void _EnterTree()
    {
        ScreenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        LogTool.DebugLogDump(osName);
        int xStart;
        int yStart;
        if (osName == "Windows")
        {
            WindowSize = new Vector2I(1920, 1080);
            DisplayServer.WindowSetSize(WindowSize);
            /* window at the center of screen */
            xStart = (ScreenSize.X - DisplayServer.WindowGetSize().X) / 2;
            yStart = (ScreenSize.Y - DisplayServer.WindowGetSize().Y) / 2;
            DisplayServer.WindowSetPosition(new Vector2I(xStart, yStart));
        }
        else if (osName == "Android")
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            WindowSize = (Vector2I)GetViewportRect().End;
            LogTool.DebugLogDump(WindowSize.ToString());
        }

        SettingContainer = GetNodeOrNull<CanvasLayer>("SettingContainer");
        if (SettingContainer == null)
        {
            LogTool.DebugLogDump("SettingContainer not found!");
            return;
        }
        SettingContainer.Visible = false;
        xStart = WindowSize.X / 2;
        yStart = WindowSize.Y / 2;
        SettingContainer.Offset = new Vector2I(xStart, yStart);

        UserInterfaceContainer = GetNodeOrNull<CanvasLayer>("UserInterfaceContainer");
        if (UserInterfaceContainer == null)
        {
            LogTool.DebugLogDump("UserInterfaceContainer not found!");
            return;
        }
        UserInterfaceContainer.GetNodeOrNull<TouchScreenButton>("SettingOpenButton").Position = new Vector2(WindowSize.X  * 13/15, WindowSize.Y * 2/15);
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