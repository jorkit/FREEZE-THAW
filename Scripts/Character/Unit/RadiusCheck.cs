using Godot;
using System.Collections.Generic;
using FreezeThaw.Utils;
public partial class RadiusCheck : Area2D
{
    public List<Survivor> SurvivorsInArea { set; get; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SurvivorsInArea = new List<Survivor>();
        CollisionLayer = 0;
        CollisionMask = 4;
        BodyEntered += BodyEnteredHandler;
        BodyExited += BodyExitedHandler;
    }

    private void BodyEnteredHandler(Node2D body)
    {
        if (body == null)
        {
            LogTool.DebugLogDump("Enter body is null!");
            return;
        }
        LogTool.DebugLogDump(body.GetType() + " enter!!!!!!");
        if (body.GetType().BaseType == typeof(Survivor) || body.GetType().BaseType.BaseType == typeof(Survivor))
        {
            SurvivorsInArea.Add((Survivor)body);
        }
    }

    private void BodyExitedHandler(Node2D body)
    {
        if (body == null)
        {
            LogTool.DebugLogDump("Exit body is null!");
            return;
        }
        LogTool.DebugLogDump(body.GetType() + " exit!!!!!!");
        if (body.GetType().BaseType == typeof(Survivor) || body.GetType().BaseType.BaseType == typeof(Survivor))
        {
            SurvivorsInArea.Remove((Survivor)body);
        }
    }
}
