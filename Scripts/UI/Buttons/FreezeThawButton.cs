using Godot;
using System;
using FreezeThaw.Utils;

public partial class FreezeThawButton : TouchScreenButton
{
	public bool CanBePressed;
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
        CanBePressed = true;
        Pressed += PressedHandle;

        /* set the position according to WindowSize */
        Position = new Vector2(UIControler.WindowSize.X * 13 / 15, UIControler.WindowSize.Y * 1 / 2);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (_uiContainer.character.GetType().BaseType == typeof(Monster) || _uiContainer.character.GetType().BaseType.BaseType == typeof(Monster))
        {
            /* detective if there are freezed Survivors */
            if (_uiContainer.character.GetCurrentState() < CharacterStateEnum.Attack && ((Monster)_uiContainer.character).CheckFreezed() == true)
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
            /* detective if there are sealed Survivor */
            if (_uiContainer.character.GetCurrentState() < CharacterStateEnum.Attack)
            {
                CanBePressed = true;
                if (((Survivor)_uiContainer.character).CheckSealed() == true)
                {
                    /* change button sprite */
                }
                else
                {
                    /* change button sprite */
                }
            }
        }
    }

	public void PressedHandle()
	{
        if (CanBePressed == false  || (IsMultiplayerAuthority() == false && _uiContainer.character.GetType().BaseType.BaseType == typeof(Character)))
        {
            return;
        }
        LogTool.DebugLogDump("FTB pressed!");
        if (NetworkControler.IsMultiplayer == true)
        {
            if (NetworkControler.MultiplayerApi.IsServer() == false)
            {
                var rpcRes = Rpc("PressedHandleRpc");
                if (rpcRes != Error.Ok)
                {
                    LogTool.DebugLogDump("PressedHandleRpc Failed! " + rpcRes.ToString());
                }
            }
            else
            {
                _uiContainer.character.FreezeThawButtonPressedHandle();
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
        LogTool.DebugLogDump(GetMultiplayerAuthority().ToString() + " receive FreezeThaw CMD from " + NetworkControler.MultiplayerApi.GetRemoteSenderId());
        _uiContainer.character.FreezeThawButtonPressedHandle();
    }
}
