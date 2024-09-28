using FreezeThaw.Utils;
using Godot;
using System;

public partial class SceneOfMatchStartLoading : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/MatchStartLoading/MatchStartLoading.tscn";
    }
    int i = 0;
    public override void Update(double delta)
    {
        if (i < 50)
        {
            LogTool.DebugLogDump("MatchStartLoading play");
            i++;
            return;
        }
        SceneFSM.PreStateChange(SceneFsm, SceneStateEnum.MatchMain, true);
    }
    public override bool EnterCondition()
    {
        if (SceneFsm.PreState != SceneStateEnum.MatchStartLoading)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter");
        var source = ResourceLoader.Load<PackedScene>(Path);
        if (source == null)
        {
            LogTool.DebugLogDump("source not found");
            return;
        }
        var scene = source.InstantiateOrNull<MatchStartLoading>();
        if (scene == null)
        {
            LogTool.DebugLogDump("scene not found");
            return;
        }
        BigBro.bigBro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.MatchStartLoading)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;

    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<MatchStartLoading>("MatchStartLoading");
        if (node != null)
        {
            BigBro.bigBro.RemoveChild(node);
        }
    }
}
