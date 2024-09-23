using Godot;
using System;

public partial class AttackButton : TouchScreenButton
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += PressedHandler;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressedHandler()
    {
        GD.Print("ATB pressed!");
    }
}
