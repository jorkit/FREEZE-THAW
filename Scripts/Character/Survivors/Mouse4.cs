using Godot;
using System;

public partial class Mouse4 : Survivor
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 500f;
        Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/Normal/Slingshot.tscn");
    }
}
