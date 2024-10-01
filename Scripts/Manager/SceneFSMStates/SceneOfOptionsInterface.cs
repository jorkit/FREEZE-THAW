using FreezeThaw.Utils;
using Godot;
using System;
using System.IO;

public partial class SceneOfOptionsInterface : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/OptionsInterface/OptionsInterface.tscn";
    }
    int i = 0;
    public override void Update(double delta)
    {
    }
    public override bool EnterCondition()
    {
        if (SceneFsm.PreState != SceneStateEnum.OptionsInterface)
        {
            return false;
        }
        //LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        //LogTool.DebugLogDump(Name + " OnEnter");
        var source = ResourceLoader.Load<PackedScene>(Path);
        if (source == null)
        {
            LogTool.DebugLogDump("source not found");
            return;
        }
        var scene = source.InstantiateOrNull<OptionsInterface>();
        if (scene == null)
        {
            LogTool.DebugLogDump("scene not found");
            return;
        }
        BigBro.bigBro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.OptionsInterface)
        {
            return false;
        }
        //LogTool.DebugLogDump(Name + " ExitCondition");

        return true;

    }
    public override void OnExit()
    {
        //LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<OptionsInterface>("OptionsInterface");
        node?.QueueFree();
    }
}
