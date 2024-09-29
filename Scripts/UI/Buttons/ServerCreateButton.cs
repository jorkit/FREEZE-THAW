using FreezeThaw.Utils;
using Godot;
using System;
using System.IO;

public partial class ServerCreateButton : TouchScreenButton
{
    private bool CanBePressed;
    private OptionContainer _optionContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _optionContainer = GetParentOrNull<OptionContainer>();
        if (_optionContainer == null)
        {
            LogTool.DebugLogDump("OptionContainer not found!");
            return;
        }
        Pressed += PressedHandler;
        CanBePressed = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressedHandler()
    {
        if (CanBePressed == false)
        {
            return;
        }
        LogTool.DebugLogDump(Name + " pressed!");
        BigBro.MultiplayerApi = Multiplayer;
        if (BigBro.MultiplayerServerInit() == false)
        {
            LogTool.DebugLogDump("MultiplayerServerInit faild!");
            return;
        }
        CanBePressed = false;

        /* Scene change */
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.WaitingHall, true);
    }
}
