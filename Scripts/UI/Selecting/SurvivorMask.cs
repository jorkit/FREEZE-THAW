using FreezeThaw.Utils;
using Godot;
using System;

public partial class SurvivorMask : Polygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        var position = ToLocal((Vector2)@event.Get("position"));
        if (@event.IsPressed() && Geometry2D.IsPointInPolygon(position, Polygon))
        {
            var survivorList = UIControler.SelectingContainer.GetNodeOrNull<SelectList>("SelectingArea/SubViewport/SurvivorList");
            if (survivorList == null)
            {
                LogTool.DebugLogDump("SurvivorList not found!");
                return;
            }
            survivorList.Visible = true;
            UIControler.SelectingContainer.Visible = true;
        }
    }
}
