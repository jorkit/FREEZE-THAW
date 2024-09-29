using Godot;
using System;
using FreezeThaw.Utils;
public partial class AttackButton : TouchScreenButton
{
    private bool CanBePressed;
    private UIContainer _uiContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
            LogTool.DebugLogDump("UIContainer not found!");
            return;
        }
        Pressed += PressedHandler;
        CanBePressed = true;

        /* set the position according to WindowSize */
        Position = new Vector2(BigBro.windowSize.X * 12 / 15, BigBro.windowSize.Y * 3 / 4);
 
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_uiContainer.character.GetCurrentState() >= CharacterStateEnum.Attack)
        {
            CanBePressed = false;
        }
        else
        {
            CanBePressed = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventJoypadButton && @event.IsPressed())
        {
            if (Input.IsJoyButtonPressed(0, JoyButton.A))
            {
                PressedHandler();
            }
        }
    }

    public void PressedHandler()
    {
        if (!CanBePressed || IsMultiplayerAuthority() == false)
        {
            return;
        }
        LogTool.DebugLogDump("ATB pressed!");
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
            _uiContainer.character.AttackButtonPressedHandle();
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void PressedHandleRpc()
    {
        LogTool.DebugLogDump(GetMultiplayerAuthority().ToString() + " receive Attack from " + BigBro.MultiplayerApi.GetRemoteSenderId());
        _uiContainer.character.AttackButtonPressedHandle();
    }
}
