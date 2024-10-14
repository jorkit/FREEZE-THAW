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
        //LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        //LogTool.DebugLogDump(Name + " OnEnter");
        var scene = ResourceLoader.Load<PackedScene>(Path).InstantiateOrNull<GameOpenLoading>();
        if (scene == null)
        {
            LogTool.DebugLogDump("Scene instantiate faild!");
            return;
        }
        BigBro.bigBro.AddChild(scene);
        BigBro.bigBro.MoveChild(scene, 0);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.GameOpenLoading)
        {
            return false;
        }
        //LogTool.DebugLogDump(Name + " ExitCondition");

        return true;

    }
    public override void OnExit()
    {
        //LogTool.DebugLogDump(Name + " OnExit");
        var node = BigBro.bigBro.GetNodeOrNull<GameOpenLoading>("GameOpenLoading");
        node?.QueueFree();
    }
}
