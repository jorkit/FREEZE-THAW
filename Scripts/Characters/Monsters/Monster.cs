using Godot;
using System.Runtime.InteropServices;
using static Survivor;

public partial class Monster : Character
{
    public new float SPEED = 500f; // 移动速度

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        SPEED = 500f;
        CharacterState = FSMState.CharacterStateEnum.Idle;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override Vector2 GetDirection()
    {
        return Joystick.GetCurPosition();
    }
}
