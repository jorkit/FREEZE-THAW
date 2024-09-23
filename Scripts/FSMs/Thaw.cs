using Godot;
using System;
using static Survivor;

public partial class Thaw : FSMState
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
        FSMState.CharacterStateChange(character, CharacterStateEnum.Idle);
    }

    public override bool EnterCondition()
    {
        if (((Survivor)character).CharacterState != CharacterStateEnum.Thaw)
        {
            return false;
        }
        GD.Print("Thaw EnterCondition!");
        return true;
    }
    public override void OnEnter()
    {
        GD.Print("Thaw OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Thaw);
    }
    public override bool ExitCondition()
    {
        if (((Survivor)character).CharacterState == CharacterStateEnum.Thaw)
        {
            return false;
        }
        GD.Print("Thaw ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Thaw OnExit!");
    }
}
