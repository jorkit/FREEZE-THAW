using Godot;
using System;

public partial class Run : FSMState
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
        if (character == null)
        {
            GD.Print("character not found!");
            return;
        }
        var velocity = character.GetDirection();
        if (velocity == new Vector2(0, 0))
        {
            var left = Input.IsActionPressed("ui_left");
            var right = Input.IsActionPressed("ui_right");
            var up = Input.IsActionPressed("ui_up");
            var down = Input.IsActionPressed("ui_down");

            if (left)
            {
                velocity.X--;
            }
            if (right)
            {
                velocity.X++;
            }
            if (up)
            {
                velocity.Y--;
            }
            if (down)
            {
                velocity.Y++;
            }
        }
        if (character.GetType() == typeof(Monster))
        {
            character.Velocity = velocity.Normalized() * (float)delta * ((Monster)character).SPEED;
        }
        else if(character.GetType() == typeof(Survivor))
        {
            character.Velocity = velocity.Normalized() * (float)delta * ((Survivor)character).SPEED;
        }

        /* get Collide info */
        var collision_info = character.MoveAndCollide(character.Velocity);
        if (collision_info != null)
        {
            var collider = collision_info.GetCollider();
            GD.Print("COlliding!" + collider.GetType());
            if (collision_info.GetCollider().GetType() == typeof(Survivor))
            {
                FSMState.CharacterStateChange((Survivor)collider, FSMState.CharacterStateEnum.Freezing);
            }
            else if (collision_info.GetCollider().GetType() != typeof(Survivor))
            {
                FSMState.CharacterStateChange((Survivor)character, FSMState.CharacterStateEnum.Freezing);
            }
        }
    }

    public override bool EnterCondition()
    {
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState > CharacterStateEnum.Run)
        {
            return false;
        }
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState > CharacterStateEnum.Run)
        {
            return false;
        }
        if (Joystick.GetCurPosition() == new Vector2(0, 0) && Input.GetAxis("ui_left", "ui_right") == 0 && Input.GetAxis("ui_up", "ui_down") == 0)
        {
            return false;
        }
        GD.Print(character.Name);
        GD.Print(character.CharacterState);
        GD.Print("Run EnterCondition!");

        return true;
    }
    public override void OnEnter()
	{
        GD.Print("Run OnEnter!");
        FSMState.CharacterStateChange(character, CharacterStateEnum.Run);
    }
    public override bool ExitCondition()
    {
        if (character.GetType() == typeof(Survivor) && ((Survivor)character).CharacterState == CharacterStateEnum.Run)
        {
            return false;
        }
        if (character.GetType() == typeof(Monster) && ((Monster)character).CharacterState == CharacterStateEnum.Run)
        {
            return false;
        }
        GD.Print("Run ExitCondition!");

        return true;
    }
    public override void OnExit()
    {
        GD.Print("Run OnExit!");
    }
}
