using Godot;
using System;
using System.Security.Cryptography;

public partial class Manager : Node
{
    public static Monster monster;
    public static Godot.Collections.Array<Survivor> survivors;
    public static Character Player;
    public static Vector2I screenSize;
    public static Vector2I windowSize;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        screenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        GD.Print(osName);
        if (osName == "Windows")
        {
            windowSize = new Vector2I(1120, 680);
            DisplayServer.WindowSetSize(windowSize);
            GD.Print(windowSize);
            var xStart = (screenSize.X - DisplayServer.WindowGetSize().X) / 2;
            var yStart = (screenSize.Y - DisplayServer.WindowGetSize().Y) / 2;
            DisplayServer.WindowSetPosition(new Vector2I(xStart, yStart));
        }
        else if (osName == "Android")
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            windowSize = DisplayServer.WindowGetSize();
            GD.Print(windowSize);
        }
        //GetNode<Monster>("/root/Main/Monster").SetScript(ResourceLoader.Load("res://Scripts/Characters/Monsters/AI.cs"));
        //GD.Print("set script");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
