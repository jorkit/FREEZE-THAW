using FreezeThaw.Utils;
using Godot;
using System;

public partial class ReadyButton : TextureButton
{
	private static bool CanBePressed { set; get; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += PressedHandler;
		CanBePressed = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PressedHandler()
	{
		if (CanBePressed == false)
		{
			return;
		}
		CanBePressed = false;
		PlayerControler.PlayerContainer.PlayerReady(NetworkControler.Id, !NetworkControler.ReadyStatus);
    }

	public void ReadyFireUpdate(bool ready)
	{
		if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == true)
		{
            Rpc("ReadyFireUpdateRpc", ready);
        }
    }

	[Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private async void ReadyFireUpdateRpc(bool ready)
	{
        Color readyButtonSelfModulate = SelfModulate;
        if (ready)
        {
            readyButtonSelfModulate.A += (float)0.1;
        }
        else
        {
            readyButtonSelfModulate.A -= (float)0.1;
        }
        SelfModulate = readyButtonSelfModulate;
		/* wait for ReadyStatus update */
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        CanBePressed = true;
    }
}
