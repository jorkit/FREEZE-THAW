using FreezeThaw.Utils;
using Godot;
using System;

public abstract partial class Survivor : Character
{
    protected PackedScene Bullet;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Speed = 400f;
    }

    public override void Attack()
    {
        var bullet = Bullet.Instantiate<Bullet>();
        var direction = GetNodeOrNull<AttackButton>("UIContainer/AttackButton").Direction;
        bullet.Direction = direction;
        bullet.GlobalPosition = Position + direction * 60;
        bullet.OwnerId = Name;
        BigBro.bigBro.AddChild(bullet);
    }

    public override void FreezeThawButtonPressedHandle()
    {
        /* If there is a Sealed Survivor, Freeing it firstly */
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
            if (survivor.GetCurrentState() == CharacterStateEnum.Sealed)
            {
                Fsm.PreStateChange(CharacterStateEnum.Freeing, false);
                return;
            }
        }

        /* There are no any Sealed Survivors */
        if (GetCurrentState() == CharacterStateEnum.Freezed)
        {
            Fsm.PreStateChange(CharacterStateEnum.Thawing, false);
        }
        else
        {
            Fsm.PreStateChange(CharacterStateEnum.Freezing, false);
        }
    }

    public bool CheckSealed()
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
            if (survivor.GetCurrentState() == CharacterStateEnum.Sealed)
            {
                return true;
            }
        }

        return false;
    }
}
