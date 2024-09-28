using FreezeThaw.Utils;
using Godot;
using System;

public partial class Sandworm : Monster
{

    public override void _EnterTree()
    {
        base._EnterTree();
    }
    public override void _Ready()
    {
        base._Ready();
        Speed = 550f;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void Attack()
    {
        base.Attack();
    }
}
