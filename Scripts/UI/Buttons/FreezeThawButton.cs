using Godot;
using System;
using FreezeThaw.Utils;

public partial class FreezeThawButton : TouchScreenButton
{
	bool CanPress;
	Survivor survivor;
    Node survivors;
	private UIContainer _uiContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
			LogTool.DebugLogDump("UIContainer not found!");
        }
        CanPress = false;
        Pressed += PressedHandler;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PressedHandler()
	{
       
	}
}
