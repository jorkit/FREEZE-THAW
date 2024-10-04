using FreezeThaw.Utils;
using Godot;
using System;

public partial class Sandworm : Monster
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 550f;
    }
}
