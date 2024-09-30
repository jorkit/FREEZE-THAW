using Godot;
using System;
using FreezeThaw.Utils;

public partial class FreezeThawButton : TouchScreenButton
{
	private bool CanBePressed;
	Survivor survivor;
    Node survivors;
	private UIContainer _uiContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
			LogTool.DebugLogDump("UIContainer not found!");
        }
        CanBePressed = false;
        Pressed += PressedHandle;

        /* set the position according to WindowSize */
        Position = new Vector2(BigBro.windowSize.X * 13 / 15, BigBro.windowSize.Y * 1 / 2);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (_uiContainer.character.GetType().BaseType == typeof(Monster))
        {
            /* detective if there are freezing Survivors */
            if (_uiContainer.character.GetCurrentState() < CharacterStateEnum.Attack)
            {
                CanBePressed = true;
            }
            else
            {
                CanBePressed = false;
            }
        }
        else
        {
            if (_uiContainer.character.GetCurrentState() < CharacterStateEnum.Attack)
            {
                CanBePressed = true;
            }
            else
            {
                CanBePressed = false;
            }
        }
    }

	public void PressedHandle()
	{
        if (CanBePressed == false)
        {
            return;
        }
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == false)
            {
                var rpcRes = Rpc("PressedHandleRpc");
                if (rpcRes != Error.Ok)
                {
                    LogTool.DebugLogDump("PressedHandleRpc Failed! " + rpcRes.ToString());
                }
            }
        }
        else
        {
            _uiContainer.character.FreezeThawButtonPressedHandle();
        }
	}

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    private void PressedHandleRpc()
    {
        LogTool.DebugLogDump(GetMultiplayerAuthority().ToString() + " receive FreezeThaw CMD from " + BigBro.MultiplayerApi.GetRemoteSenderId());
        _uiContainer.character.FreezeThawButtonPressedHandle();
    }
}
