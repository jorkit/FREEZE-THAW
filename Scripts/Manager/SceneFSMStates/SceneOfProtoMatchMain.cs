using FreezeThaw.Utils;
using Godot;
using System;

public partial class SceneOfProtoMatchMain : SceneFSMState
{
    public override void _Ready()
    {
        base._Ready();
        Path = "res://Scenes/Terminal/MatchMain/ProtoMatchMain/ProtoMatchMain.tscn";
    }

    public override void Update(double delta)
    {

    }
    public override bool EnterCondition()
    {
        /* if MatchMain ready, return ture */
        if (SceneFsm.PreState != SceneStateEnum.MatchMain)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " EnterCondition");

        return true;
    }
    public override void OnEnter()
    {
        LogTool.DebugLogDump(Name + " OnEnter");
        var scene = ResourceLoader.Load<PackedScene>(Path).InstantiateOrNull<ProtoMatchMain>();
        if (scene == null)
        {
            LogTool.DebugLogDump("Scene instantiate faild");
        }
        SceneFsm.bigbro.AddChild(scene);
    }
    public override bool ExitCondition()
    {
        if (SceneFsm.PreState == SceneStateEnum.MatchMain)
        {
            return false;
        }
        LogTool.DebugLogDump(Name + " ExitCondition");

        return true;
    }
    public override void OnExit()
    {
        LogTool.DebugLogDump(Name + " OnExit");
        var node = SceneFsm.bigbro.GetNode<ProtoMatchMain>("ProtoMatchMain");
        if (node != null)
        {
            SceneFsm.bigbro.RemoveChild(node);
        }
    }
}
