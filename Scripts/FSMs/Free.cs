using Godot;
using System;
using static Survivor;

public partial class Free : FSMState
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
        GD.Print("Play animation");
        ((Survivor)character).CharacterState = CharacterStateEnum.Idle;
    }

    public override bool EnterCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Free)
        {
            return false;
        }
        GD.Print("Free EnterCondition!");
        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Free OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Free);
    }
    public override bool ExitCondition()
    {
        if (((Survivor)character).CharacterState == CharacterStateEnum.Free)
        {
            return false;
        }
        GD.Print("Free ExitCondition!");
        return true;
    }
    public override void OnExit()
    {
        GD.Print("Free OnExit!");
    }
}
