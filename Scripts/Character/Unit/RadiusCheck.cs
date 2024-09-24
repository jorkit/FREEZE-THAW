using Godot;
using System;
using FreezeThaw.Utils;
public partial class RadiusCheck : Area2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyShapeEntered += BodyShapeEnteredHandler;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void BodyShapeEnteredHandler(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
    {
        if (body == null)
        {
            LogTool.DebugLogDump("Enter body is null!");
            return;
        }
        LogTool.DebugLogDump(body.GetType() + " enter!!!!!!");
    }
}
