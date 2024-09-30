using Godot;
using System;

public abstract partial class Survivor : Character
{
    protected PackedScene Bullet;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Speed = 400f;
        GetNodeOrNull<Polygon2D>("BulletDirection").Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Attack()
    {
        var bullet = Bullet.Instantiate<Bullet>();
        bullet.SetDirection(GetNodeOrNull<AttackButton>("UIContainer/AttackButton").direction);
        bullet.GlobalPosition = GetNode<Marker2D>("BulletBornPosition").GlobalPosition;
        GetParent().AddChild(bullet);
    }
}
