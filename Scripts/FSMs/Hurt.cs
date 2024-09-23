using Godot;
using System;

public partial class Hurt : FSMState
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
        GD.Print("Animatiton play");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Idle);
    }

    public override bool EnterCondition()
    {
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState != CharacterStateEnum.Hurt)
        {
            return false;
        }
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState != CharacterStateEnum.Hurt)
        {
            return false;
        }

        GD.Print("Hurt EnterCondition!");

        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Hurt OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Hurt);
    }
    public override bool ExitCondition()
    {
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState == CharacterStateEnum.Hurt)
        {
            return false;
        }
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState == CharacterStateEnum.Hurt)
        {
            return false;
        }
        GD.Print("Hurt ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Hurt OnExit!");
    }
}
