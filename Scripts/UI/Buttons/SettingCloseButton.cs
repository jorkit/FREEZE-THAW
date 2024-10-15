using Godot;
using System;

public partial class SettingCloseButton : TouchScreenButton
{
    bool CanBePressed;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CanBePressed = true;
        Pressed += PressedHandle;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressedHandle()
    {
        if (CanBePressed == false)
        {
            return;
        }
        UIControler.SettingContainer.Hide();
    }
}
