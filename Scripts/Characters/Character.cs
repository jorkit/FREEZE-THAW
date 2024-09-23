using Godot;
using static Survivor;

public abstract partial class Character : CharacterBody2D
{
    public float SPEED; // 移动速度
    public FSMState.CharacterStateEnum CharacterState { set; get; }

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

    public abstract Vector2 GetDirection();
}
