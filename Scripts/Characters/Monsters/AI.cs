using Godot;

public partial class AI : Monster
{
    public new float SPEED = 200f; // 移动速度
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        GD.Print("AI Ready!!!!");
        CharacterState = FSMState.CharacterStateEnum.Run;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    public new Vector2 GetDirection()
    {
        var survivor = Manager.Player;
        if (survivor == null)
        {
            GD.Print("Player not found!");
        }
        return Manager.Player.Position - Position;
    }
}
