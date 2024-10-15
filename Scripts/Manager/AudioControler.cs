using FreezeThaw.Utils;
using Godot;
using System;

public partial class AudioControler : Node
{
	public enum AudioBusEnum
	{
		Master = 0,
		Effects,
		MAX
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < (int)AudioBusEnum.MAX; i++)
		{
			SetAudioVolume((AudioBusEnum)i, (float)SettingControler.Settings[Enum.GetName((AudioBusEnum)i)]);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static float GetAudioBusVolume(AudioBusEnum bus)
	{
		var volume = AudioServer.GetBusVolumeDb((int)bus);

		return Mathf.DbToLinear(volume);
	}

	/* Set volume and save to config file */
	public static void SetAudioVolume(AudioBusEnum bus, float volume)
	{
		var db = Mathf.LinearToDb(volume);
        AudioServer.SetBusVolumeDb((int)bus, db);
		SettingControler.Save(Enum.GetName(SettingControler.SettingEnum.Audio), Enum.GetName(bus), volume);
    }

	/* Effects play */
	public void Attack(Node2D node, string audioName)
	{
		var audio = GetNodeOrNull<AudioStreamPlayer2D>("Attack/" + audioName);
		if (audio == null)
		{
			LogTool.DebugLogDump("audio not found!");
			return;
		}
        audio.Position = node.Position;
        audio.Play();
	}

	public void Hit(Node2D node, string audioName)
	{
        var audio = GetNodeOrNull<AudioStreamPlayer2D>("Hit/" + audioName);
        if (audio == null)
        {
            LogTool.DebugLogDump("audio not found!");
            return;
        }
		audio.Position = node.Position;
        audio.Play();
    }
}
