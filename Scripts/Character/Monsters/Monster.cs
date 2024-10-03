using FreezeThaw.Utils;
using Godot;
using System;

public abstract partial class Monster : Character
{
    public Area2D attackArea { get; set; }
    public bool Hurting { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Speed = 500f;
        BigBro.Monster = this;
        attackArea = GetNodeOrNull<Area2D>("AttackArea");
        if (attackArea == null)
        {
            LogTool.DebugLogDump("AttackArea not found!");
            return;
        }
        if (attackArea.IsConnected("body_entered", new Callable(this, "BodyEnteredHandler")) == false)
        {
            attackArea.Connect("body_entered", new Callable(this, "BodyEnteredHandler"));
        }
        attackArea.CollisionMask = 0;

        if (SelfImage.IsConnected("frame_changed", new Callable(this, "FrameChangedHandler")) == false)
        {
            SelfImage.Connect("frame_changed", new Callable(this, "FrameChangedHandler"));
        }
    }

    private void BodyEnteredHandler(Node2D body)
    {
        if (body.GetType().BaseType == typeof(Survivor) || body.GetType().BaseType.BaseType == typeof(Survivor))
        {
            LogTool.DebugLogDump("lalalalala");
            ((Survivor)body).Fsm.PreStateChange(CharacterStateEnum.Hurt, false);
        }
    }

    private void FrameChangedHandler()
    {
        if (SelfImage.Animation == "Attack")
        {
            var frameIndex = SelfImage.Frame;
            if (frameIndex == 4)
            {
                attackArea.CollisionMask = 4;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Attack()
    {
        var direction = GetNodeOrNull<AttackButton>("UIContainer/AttackButton").direction;
        attackArea.Rotation = direction.Angle();
    }

    public override void FreezeThawButtonPressedHandle()
    {

    }
}
