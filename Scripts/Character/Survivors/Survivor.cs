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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Attack()
    {
        GD.Print(GetMultiplayerAuthority() + " Attack");
        var bullet = Bullet.Instantiate<Bullet>();
        bullet.SetDirection(new Vector2(1, 0));
        GetParent().AddChild(bullet);
        bullet.GlobalPosition = GetNode<Marker2D>("BulletBornPosition").GlobalPosition;
    }
}
