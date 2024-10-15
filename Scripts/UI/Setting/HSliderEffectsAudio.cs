using FreezeThaw.Utils;
using Godot;
using System;

public partial class HSliderEffectsAudio : HSlider
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Value = AudioControler.GetAudioBusVolume(AudioControler.AudioBusEnum.Effects);
        ValueChanged += ValueChangedHandler;
	}

    private void ValueChangedHandler(double value)
    {
		AudioControler.SetAudioVolume(AudioControler.AudioBusEnum.Effects, (float)value);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
