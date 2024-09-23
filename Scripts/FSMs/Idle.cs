using Godot;
using System;

public partial class Idle : FSMState
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
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState != CharacterStateEnum.Idle)
        {
            return false;
        }
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState != CharacterStateEnum.Idle)
        {
            return false;
        }
        if (Joystick.GetCurPosition() != new Vector2(0, 0) || Input.GetAxis("ui_left", "ui_right") != 0 || Input.GetAxis("ui_up", "ui_down") != 0)
        {
            return false;
        }
        GD.Print(character.GetType());
        GD.Print("Idle EnterCondition!");

        return true;
    }
    public override void OnEnter()
	{
        GD.Print("Idle OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Idle);
	}
    public override bool ExitCondition()
    {
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState == CharacterStateEnum.Idle)
        {
            return false;
        }
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState == CharacterStateEnum.Idle)
        {
            return false;
        }
        GD.Print("Idle ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Idle OnExit!");
    }
}
