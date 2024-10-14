using FreezeThaw.Utils;
using Godot;
using System;
using static Godot.Projection;
using static PlayerContainer;

//GetNode<Monster>("/root/Main/Monster").SetScript(ResourceLoader.Load("res://Scripts/Characters/Monsters/AI.cs"));
//LogTool.DebugLogDump("set script");
public partial class BigBro : Node
{
    public static BigBro bigBro { set; get; }

    /* Scene FSM */
    public static SceneFSM SceneFSM { set; get; }

    /* UI Controler */
    public static UIControler UIControler { set; get; }

    /* Setting Controler */
    public static SettingControler SettingControler { set; get; }

    /* Player Controler */
    public static PlayerControler PlayerControler { set; get; }

    /* Multiplayer Networke Controler */
    public static NetworkControler NetworkControler { set; get; }

    /* Audio Controler */
    public static AudioControler AudioControler { set; get; }

    /* Network Contrler */
    public static NetworkControler NetworkContrler { set; get; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BigBro.bigBro = this;

        /* Check components */
        SceneFSM = GetNodeOrNull<SceneFSM>("SceneFSM");
        if (SceneFSM == null)
        {
            LogTool.DebugLogDump("SceneFSM not found!");
            return;
        }

        UIControler = GetNodeOrNull<UIControler>("UIControler");
        if (UIControler == null)
        {
            LogTool.DebugLogDump("UIControler not found!");
            return;
        }

        SettingControler = GetNodeOrNull<SettingControler>("SettingControler");
        if (SettingControler == null)
        {
            LogTool.DebugLogDump("SettingControler not found!");
            return;
        }

        PlayerControler = GetNodeOrNull<PlayerControler>("PlayerControler");
        if (PlayerControler == null)
        {
            LogTool.DebugLogDump("PlayerControler not found!");
            return;
        }

        AudioControler = GetNodeOrNull<AudioControler>("AudioControler");
        if (AudioControler == null)
        {
            LogTool.DebugLogDump("AudioControler not found!");
            return;
        }

        NetworkControler = GetNodeOrNull<NetworkControler>("NetworkControler");
        if (NetworkControler == null)
        {
            LogTool.DebugLogDump("NetworkControler not found!");
            return;
        }

        /* first scene loading */
        if (SceneFSM.SetInitState() == false)
        {
            LogTool.DebugLogDump("SceneFSM init faild!");
            return;
        }
    }
}
