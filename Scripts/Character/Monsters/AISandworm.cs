using FreezeThaw.Utils;
using Godot;
using System;

public partial class AISandworm : Sandworm
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 400f;
    }
    public override void _PhysicsProcess(double delta)
    {
        Velocity = (BigBro.Player.Position - Position).Normalized() * Speed;
        base._PhysicsProcess(delta);
    }

    public override void Attack()
    {
        base.Attack();
    }
}
