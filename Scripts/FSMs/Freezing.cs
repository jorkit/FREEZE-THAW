using Godot;
using System;

public partial class Freezing : FSMState
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
        GD.Print("Animatition play!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Freezed);
    }

    public override bool EnterCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Freezing)
        {
            return false;
        }
        GD.Print("Freezing EnterCondition!");
        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Freezing OnEnter!");
        //((Survivor)character).CollisionLayer = 2;
        FSMState.CharacterStateChange(character, CharacterStateEnum.Freezing);
    }
    public override bool ExitCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Freezed)
        {
            return false;
        }
        GD.Print("Freezing ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Freezing OnExit!");
    }
}
