using FreezeThaw.Utils;
using Godot;
using System;

public partial class SceneOfGameOpenLoading : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/GameOpenLoading/GameOpenLoading.tscn";
    }
    int i = 0;
    public override void Update(double delta)
    {
        if (i < 50)
        {
            i++;
            return;
        }
        SceneFSM.PreStateChange(SceneFsm, SceneStateEnum.OptionsInterface, true);
    }
    public override bool EnterCondition()
    {
        if (SceneFsm.PreState != SceneStateEnum.GameOpenLoading)
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
        var scene = source.InstantiateOrNull<GameOpenLoading>();
        if (scene == null)
        {
            LogTool.DebugLogDump("scene not found");
            return;
        }
        BigBro.bigBro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.GameOpenLoading)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;

    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<GameOpenLoading>("GameOpenLoading");
        node?.QueueFree();
    }
}
