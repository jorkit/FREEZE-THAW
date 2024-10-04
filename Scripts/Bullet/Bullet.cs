using FreezeThaw.Utils;
using Godot;
using System;

public abstract partial class Bullet : Area2D
{
    protected int Speed { get; set; }
    protected int Damage { get; set; }
    public Vector2 Direction { get; set; }
    private int HitScore = 5;

    public string OwnerId {  get; set; }
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
	{
        BodyEntered += new BodyEnteredEventHandler(BodyEnteredHandle);
        await ToSignal(GetTree().CreateTimer(5), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += Direction * Speed * (float)delta;
    }

    public virtual void BodyEnteredHandle(Node2D body)
    {
        if (body == null)
        {
            return;
        }
        if (BigBro.IsMultiplayer == true)
        {
            if (body.GetType().BaseType == typeof(Monster))
            {
                QueueFree();
                if (BigBro.MultiplayerApi.IsServer() == true)
                {
                    var playerContainer = BigBro.PlayerContainer;
                    if (playerContainer != null)
                    {
                        playerContainer.ChangeScore(OwnerId, HitScore);
                    }
                }
            }
        }
        else
        {
            if (body.GetType().BaseType == typeof(Monster) || body.GetType().BaseType.BaseType == typeof(Monster))
            {
                QueueFree();
                var playerContainer = BigBro.PlayerContainer;
                if (playerContainer != null)
                {
                    playerContainer.ChangeScore(OwnerId, HitScore);
                }
            }
        }
    }
}
