using FreezeThaw.Utils;
using Godot;
using System;

public partial class HSliderMasterAudio : HSlider
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Value = AudioControler.GetAudioBusVolume(AudioControler.AudioBusEnum.Master);
        ValueChanged += ValueChangedHandler;
    }

    private void ValueChangedHandler(double value)
    {
        AudioControler.SetAudioVolume(AudioControler.AudioBusEnum.Master, (float)value);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
