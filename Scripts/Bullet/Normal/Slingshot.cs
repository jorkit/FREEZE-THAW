using Godot;
using System;

public partial class Slingshot : Bullet
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Speed = 200;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta);	
    }

	public override void BodyEnteredHandle(Node2D body)
	{
		base.BodyEnteredHandle(body);
	}
}
