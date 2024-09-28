using FreezeThaw.Utils;
using Godot;
using System;
using System.IO;

public partial class SceneOfWaitingHall : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/WaitingHall/WaitingHall.tscn";
    }

    public override void Update(double delta)
    {

    }
    public override bool EnterCondition()
    {
        /* if MatchMain ready, return ture */
        if (SceneFsm.PreState != SceneStateEnum.WaitingHall)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter");
        var scene = ResourceLoader.Load<PackedScene>(Path).InstantiateOrNull<WaitingHall>();
        if (scene == null)
        {
            LogTool.DebugLogDump("Scene instantiate faild");
        }
        BigBro.bigBro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.WaitingHall)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<WaitingHall>("WaitingHall");
        if (node != null)
        {
            BigBro.bigBro.RemoveChild(node);
        }
    }
}
