using Godot;
using System;

public abstract partial class Monster : Character
{
    public bool Hurting { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Speed = 500f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Attack()
    {
    }

    public override void FreezeThawButtonPressedHandle()
    {

    }
}
