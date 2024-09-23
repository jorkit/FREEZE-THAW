using Godot;
using System.Numerics;

public partial class UiContainer : CanvasLayer
{
    private sbyte ondraging;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var joystick = GetNodeOrNull<Joystick>("Joystick");
        if (joystick == null)
        {
            GD.Print("Joystick not found!");
        }
        var attackButton = GetNodeOrNull<AttackButton>("AttackButton");
        if (attackButton == null)
        {
            GD.Print("AttackButton not found!");
        }
        var freezeThawButton = GetNodeOrNull<FreezeThawButton>("FreezeThawButton");
        if (freezeThawButton == null)
        {
            GD.Print("freezeThawButton not found!");
        }
        joystick.Position = new Vector2I(0, 0);
        attackButton.Position = new Godot.Vector2(Manager.windowSize.X * 12 / 15, Manager.windowSize.Y * 10 / 15);
        freezeThawButton.Position = new Godot.Vector2(Manager.windowSize.X * 13 / 15, Manager.windowSize.Y * 6 / 15);

        //DisplayServer.WindowSetRectChangedCallback(); 
    }

    public override void _Input(InputEvent @event)
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
