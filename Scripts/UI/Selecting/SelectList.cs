using FreezeThaw.Utils;
using Godot;
using System;

public partial class SelectList : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventScreenTouch && @event.IsPressed())
		{
			var position = ToLocal((Vector2)@event.Get("position"));
            if (position.X > 500 && position.X < 1000)
			{
				UIControler.SelectingContainer.Visible = false;
			}
		}
    }
}
