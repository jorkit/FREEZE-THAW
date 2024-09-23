using Godot;
using System;

public partial class Survivor : Character
{
    public float SPEED; // 移动速度
    public FSMState.CharacterStateEnum CharacterState { set; get; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SPEED = 300f;
        CharacterState = FSMState.CharacterStateEnum.Idle;
        //Manager.survivors.Add(this);
        Manager.Player = this;
        GD.Print("Survivor Add!!!!");
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
