using FreezeThaw.Utils;
using Godot;

public partial class ClientJoinButton : Button
{
    private static OptionContainer _optionContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _optionContainer = GetParentOrNull<OptionContainer>();
        if (_optionContainer == null)
        {
            LogTool.DebugLogDump("OptionContainer not found!");
            return;
        }
        Pressed += PressedHandler;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressedHandler()
    {
        if (NetworkControler.Connected == true)
        {
            LogTool.DebugLogDump("Connected, Can't Be Pressed");
            return;
        }
        LogTool.DebugLogDump(Name + " pressed!");
        var serverAddress = _optionContainer.GetNode<TextEdit>("ServerAddress");
        if (System.String.IsNullOrWhiteSpace(serverAddress.Text) == true)
        {
            serverAddress.Text = "Please input server IP or Domain";
            return;
        }
        NetworkControler.MultiplayerApi = Multiplayer;
        BigBro.NetworkControler.MultiplayerClientInit(serverAddress);
    }

    public static void ClientJoinFaild()
    {
        _optionContainer.GetNode<TextEdit>("ServerAddress").Text = "Please check server IP or Domain";
    }
}
