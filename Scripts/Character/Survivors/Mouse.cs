using Godot;
using System;

public partial class Mouse : Survivor
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 500f;
        Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/Normal/Slingshot.tscn");
    }

    public override void Attack()
    {
        base.Attack();
    }
}
