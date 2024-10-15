using FreezeThaw.Utils;
using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;

public partial class SettingOpenButton : TouchScreenButton
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
        UIControler.SettingContainer.Popup();
    }
}
