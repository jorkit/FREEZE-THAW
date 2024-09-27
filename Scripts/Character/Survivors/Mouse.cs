using Godot;
using System;

public partial class Mouse : Survivor
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 400f;
        Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/Normal/Slingshot.tscn");
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override Vector2 GetDirection()
    {
        return Joystick.GetCurPosition();
    }

    public override void Attack()
    {
        base.Attack();
    }
}
