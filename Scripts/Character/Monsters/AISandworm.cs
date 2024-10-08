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
}
