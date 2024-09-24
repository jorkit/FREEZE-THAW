using Godot;
using System;
using FreezeThaw.Utils;
public partial class AttackButton : TouchScreenButton
{
    private bool CanBePressed;
    private UIContainer _uiContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
            LogTool.DebugLogDump("UIContainer not found!");
            return;
        }
        Pressed += PressedHandler;
        CanBePressed = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_uiContainer.character.GetCurrentState() >= CharacterStateEnum.Attack)
        {
            CanBePressed = false;
        }
        else
        {
            CanBePressed = true;
        }
    }

    public void PressedHandler()
    {
        LogTool.DebugLogDump("ATB pressed!");
        _uiContainer.character.AttackButtonPressedHandle();
    }
}
