using FreezeThaw.Utils;
using Godot;
using Godot.Collections;
using System;
using static AudioControler;

public partial class SettingControler : Node
{
    public enum SettingEnum
    {
        Audio = 0,
        MAX
    }
    private static readonly string SettingSavePath = "user://Setting.ini";
    private static ConfigFile ConfigFile { get; set; }
    public static Dictionary Settings { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // 创建新的 ConfigFile 对象。
        ConfigFile = new ConfigFile();
        Settings = new Dictionary();

        // 从文件加载数据。
        Error err = ConfigFile.Load(SettingSavePath);

        // 如果文件没有加载，忽略它。
        if (err != Error.Ok)
        {
            LogTool.DebugLogDump("ConfigFile not found!");
            return;
        }

        // 迭代所有小节。
        foreach (String setting in ConfigFile.GetSections())
        {
            if (setting == Enum.GetName(SettingEnum.Audio))
            {
                // 获取每个小节的数据。
                for (int i = 0; i < (int)AudioBusEnum.MAX; i++)
                {
                    var effectsVolume = (float)ConfigFile.GetValue(setting, Enum.GetName((AudioBusEnum)i), 0.5);
                    Settings.Add(Enum.GetName((AudioBusEnum)i), effectsVolume);
                }
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static void Save(string section, string key, Variant value)
    {
        ConfigFile.SetValue(section, key, value);
        ConfigFile.Save(SettingSavePath);
    }
}
