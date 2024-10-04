using Godot;
using System;

public partial class AIMouse : Mouse
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 300;
    }
    public override void _PhysicsProcess(double delta)
    {
        Velocity = (BigBro.Monster.Position - Position).Normalized() * Speed;
        base._PhysicsProcess(delta);
    }

    public override void Attack()
    {
        var bullet = Bullet.Instantiate<Bullet>();
        var direction = new Vector2(1, 1);
        bullet.Direction = direction;
        bullet.GlobalPosition = Position + direction * 60;
        bullet.OwnerId = Name;
        BigBro.bigBro.AddChild(bullet);
    }
}
