using Godot;
using System;

public partial class Seal : FSMState
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
        if (((Survivor)character).CharacterState != CharacterStateEnum.Seal)
        {
            return false;
        }
        GD.Print("Seal EnterCondition!");

        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Seal OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Seal);
    }
    public override bool ExitCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Free)
        {
            return false;
        }
        GD.Print("Seal ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Seal OnExit!");
    }
}
