using FreezeThaw.Utils;
using Godot;
using System;
using System.IO;

public partial class SceneOfMatchSettlement : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/MatchSettlement/MatchSettlement.tscn";
    }

    public override void Update(double delta)
    {
    }
    public override bool EnterCondition()
    {
        if (SceneFsm.PreState != SceneStateEnum.MatchSettlement)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter");
        var scene = ResourceLoader.Load<PackedScene>(Path).InstantiateOrNull<MatchSettlement>();
        if (scene == null)
        {
            LogTool.DebugLogDump("Scene instantiate faild");
        }
        BigBro.bigBro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.MatchSettlement)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<MatchSettlement>("MatchSettlement");
        node?.QueueFree();
    }
}
