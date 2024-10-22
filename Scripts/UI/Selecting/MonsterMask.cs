using FreezeThaw.Utils;
using Godot;
using System;

public partial class MonsterMask : Polygon2D
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
            var selectingArea =WaitingHall.SelectingArea;
            if (selectingArea == null)
            {
                LogTool.DebugLogDump("selectingArea not found!");
                return;
            }
            var monsterList = selectingArea.GetNodeOrNull<SelectList>("SubViewport/MonsterList");
            if (monsterList == null)
            {
                LogTool.DebugLogDump("MonsterList not found!");
                return;
            }
            monsterList.Visible = true;
            selectingArea.MouseFilter = Control.MouseFilterEnum.Stop;
        }
    }
}
