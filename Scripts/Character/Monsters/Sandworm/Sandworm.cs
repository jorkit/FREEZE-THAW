using FreezeThaw.Utils;
using Godot;
using System;

public partial class Sandworm : Character
{

    public override void _EnterTree()
    {
        base._EnterTree();
        if (BigBro.IsMultiplayer == true)
        {
            SetMultiplayerAuthority(Name.ToString().ToInt());
            Position = new Vector2(300, 300);
            if (IsMultiplayerAuthority() == false)
            {
                LogTool.DebugLogDump(GetMultiplayerAuthority().ToString());
                RemoveChild(GetNode<UIContainer>("UIContainer"));
                RemoveChild(GetNode<Camera2D>("CharacterCamera"));
            }
        }
        else
        {
            RemoveChild(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer"));
        }
    }
    public override void _Ready()
    {
        base._Ready();
        Speed = 550f;    
    }
    public override void _PhysicsProcess(double delta)
    {
        if (IsMultiplayerAuthority() == false)
        {
            return;
        }
        base._PhysicsProcess(delta);
    }

    public override Vector2 GetDirection()
    {
        return Joystick.GetCurPosition();
    }
}
