using FreezeThaw.Utils;
using Godot;
using System;

public partial class SelectList : Node2D
{
	private static bool Draging {  get; set; }
	private static float DragStart {  get; set; }

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
		if (GetViewportRect().HasPoint((Vector2)@event.Get("position")) == false && Draging == false)
		{
			return;
		}
		/* record touch position */
        if (@event is InputEventScreenTouch && @event.IsPressed())
		{
			Draging = true;
			DragStart = ToLocal((Vector2)@event.Get("position")).X;
			LogTool.DebugLogDump("lalalala");
		}
		/* release touch */
		if (@event is InputEventScreenTouch && !@event.IsPressed())
		{
			Draging = false;
			DragStart = 0;
            LogTool.DebugLogDump("lueluelue");
        }
		/* move list depends on Drag distance */
		if (@event is InputEventScreenDrag && Draging == true)
		{
            LogTool.DebugLogDump("lululululu");
			var newX = ToLocal((Vector2)@event.Get("position")).X;
            Position += new Vector2(newX - DragStart, 0);
        }
    }
}
