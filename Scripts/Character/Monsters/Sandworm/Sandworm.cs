using Godot;
using System;

public partial class Sandworm : Character
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 550f;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override Vector2 GetDirection()
    {
        return Joystick.GetCurPosition();
    }
}
