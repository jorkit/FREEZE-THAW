using Godot;
using System;

public partial class Freezed : FSMState
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
        if (((Survivor)character).CharacterState != CharacterStateEnum.Freezed)
        {
            return false;
        }
        GD.Print("Freezed EnterCondition!");

        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Freezed OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Freezed);
    }
    public override bool ExitCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Seal && ((Survivor)character).CharacterState != CharacterStateEnum.Thaw)
        {
            return false;
        }
        GD.Print("Freezed ExitCondition!");
        return true;
    }
    public override void OnExit()
    {
        GD.Print("Freezed OnExit!");
    }
}
