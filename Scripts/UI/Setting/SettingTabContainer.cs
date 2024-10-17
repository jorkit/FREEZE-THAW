using FreezeThaw.Utils;
using Godot;
using System;

public partial class SettingTabContainer : TabContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _GuiInput(InputEvent @event)
    {
		/* Consume the input event */
		if (@event.IsPressed())
		{
            GetViewport().SetInputAsHandled();
        }
    }
}
