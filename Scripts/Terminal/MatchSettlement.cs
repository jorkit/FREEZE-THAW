using FreezeThaw.Utils;
using Godot;
using System;
using System.Linq;

public partial class MatchSettlement : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        var uiContainer = GetNodeOrNull<CanvasLayer>("CanvasLayer");
        if (uiContainer == null)
        {
            LogTool.DebugLogDump("uiContainer not found!");
            return;
        }
        /* set the ui postion */
        uiContainer.Offset = new Vector2(BigBro.WindowSize.X / 3, BigBro.WindowSize.Y / 4);

        /* get id and score ui components */
        var ID_Labels = uiContainer.GetNodeOrNull<Node2D>("ID_Labels");
        if (ID_Labels == null)
        {
            LogTool.DebugLogDump("ID_Labels not found!");
            return;
        }
        var Score_Labels = uiContainer.GetNodeOrNull<Node2D>("Score_Labels");
        if (Score_Labels == null)
        {
            LogTool.DebugLogDump("Score_Labels not found!");
            return;
        }

        /* sort the player by score */
        var orderPlayers = BigBro.PlayerContainer.Players.OrderByDescending(player => player.Score);

        /* set the data */
        int i = 1;
        foreach (var player in orderPlayers)
        {
            ((Label)ID_Labels.GetChild(i)).Text = player.Id;
            ((Label)Score_Labels.GetChild(i)).Text = player.Score.ToString();
            i++;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
