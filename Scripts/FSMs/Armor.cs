using Godot;
using System;

public partial class Armor : FSMState
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Update(double delta)
    {

    }

    public override bool EnterCondition()
    {
        return false;
        GD.Print("Armor EnterCondition!");
    }
    public override void OnEnter()
    {
        GD.Print("Armor OnEnter!");
    }
    public override bool ExitCondition()
    {
        GD.Print("Armor ExitCondition!");
        return true;
    }
    public override void OnExit()
    {
        GD.Print("Armor OnExit!");
    }
}
