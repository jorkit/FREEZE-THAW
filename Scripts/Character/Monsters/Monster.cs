using FreezeThaw.Utils;
using Godot;
using System;

public abstract partial class Monster : Character
{
    private int ATTACK_SCORE = -20;
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
            ((Survivor)body).Fsm.PreStateChange(CharacterStateEnum.Hurt, false);
            BigBro.PlayerContainer.ChangeScore(body.Name, ATTACK_SCORE);
        }
    }

    private void FrameChangedHandler()
    {
        if (SelfImage.Animation == "Attack")
        {
            var frameIndex = SelfImage.Frame;
            if (frameIndex == 3)
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

    public void Sealing()
    {
        var radiusCheck = GetNodeOrNull<RadiusCheck>("RadiusCheck");
        if (radiusCheck == null)
        {
            LogTool.DebugLogDump("RadiusCheck not found!");
            return;
        }
        if (radiusCheck.SurvivorsInArea == null)
        {
            LogTool.DebugLogDump("SurvivorsInArea not found!");
            return;
        }
        foreach (var survivor in radiusCheck.SurvivorsInArea)
        {
            if (survivor.GetCurrentState() == CharacterStateEnum.Freezed)
            {
                survivor.Fsm.PreStateChange(CharacterStateEnum.Sealed, true);
            }
        }
    }

    public override void FreezeThawButtonPressedHandle()
    {
        Fsm.PreStateChange(CharacterStateEnum.Sealing, false);
    }

    public bool CheckFreezed()
    {
        var radiusCheck = GetNodeOrNull<RadiusCheck>("RadiusCheck");
        if (radiusCheck == null)
        {
            LogTool.DebugLogDump("RadiusCheck not found!");
            return false;
        }
        if (radiusCheck.SurvivorsInArea == null)
        {
            LogTool.DebugLogDump("SurvivorsInArea not found!");
            return false;
        }
        foreach (var survivor in radiusCheck.SurvivorsInArea)
        {
            if (survivor.GetCurrentState() == CharacterStateEnum.Freezed)
            {
                return true;
            }
        }

        return false;
    }
}
