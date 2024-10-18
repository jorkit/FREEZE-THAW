using FreezeThaw.Utils;
using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract partial class Bullet : Area2D
{
    protected int Speed { get; set; }
    protected int Damage { get; set; }
    public Vector2 Direction { get; set; }
    private int HitScore = 5;

    public Survivor Owner {  get; set; }
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
        
        if (body.GetType().BaseType == typeof(Monster))
        {
            BulletHitHandler();
        }
    }

    private async void BulletHitHandler()
    {
        BigBro.AudioControler.Hit(this, "SlingshotHitAudio");
        Visible = false;
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() != true)
        {
            return;
        }
        var playerContainer = PlayerControler.PlayerContainer;
        if (playerContainer != null)
        {
            playerContainer.ChangeScore(Owner.Name, HitScore);
        }
    }
}
