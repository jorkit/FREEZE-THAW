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
    public static Vector2I ScreenSize {  set; get; }
    public static Vector2I WindowSize { set; get; }

    /* Multiplayer */
    public static bool IsMultiplayer { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }
    
    /* Player */
    public static Character Player { set; get; }
    public static Monster Monster { set; get; }
    public static PlayerContainer PlayerContainer { set; get; }
    public static readonly string PlayerContainerPath = "res://Scenes/Manager/PlayerContainer.tscn";
    public static Godot.Collections.Array<Survivor> Survivors { set; get; }
    
    public static SceneFSM SceneFSM { set; get; }

    /* Audio */
    public static AudioControler AudioControler { set; get; }

    public override void _EnterTree()
    {
        base._EnterTree();
        IsMultiplayer = false;
        ScreenSize = DisplayServer.ScreenGetSize();
        var osName = OS.GetName();
        LogTool.DebugLogDump(osName);
        if (osName == "Windows")
        {
            WindowSize = new Vector2I(1920, 1080);
            DisplayServer.WindowSetSize(WindowSize);
            /* window at the center of screen */
            var xStart = (ScreenSize.X - DisplayServer.WindowGetSize().X) / 2;
            var yStart = (ScreenSize.Y - DisplayServer.WindowGetSize().Y) / 2;
            DisplayServer.WindowSetPosition(new Vector2I(xStart, yStart));
        }
        else if (osName == "Android")
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            WindowSize = DisplayServer.WindowGetSize();
            LogTool.DebugLogDump(WindowSize.ToString());
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BigBro.bigBro = this;
        SceneFSM = GetNodeOrNull<SceneFSM>("SceneFSM");
        if (SceneFSM == null)
        {
            LogTool.DebugLogDump("SceneFSM not found!");
            return;
        }
        SceneFSM.SetInitState();

        AudioControler = GetNodeOrNull<AudioControler>("AudioControler");
        if (AudioControler == null)
        {
            LogTool.DebugLogDump("AudioControler not found!");
            return;
        }
    }

    public static bool MultiplayerServerInit()
    {
        BigBro.IsMultiplayer = true;
        BigBro.Peer = new();
        BigBro.Spawner = new();

        var CreateServerResult = BigBro.Peer.CreateServer(7788);
        if (CreateServerResult != Error.Ok)
        {
            LogTool.DebugLogDump("Server create failed!");
            return false;
        }
        BigBro.MultiplayerApi.MultiplayerPeer = BigBro.Peer;

        BigBro.CreatePlayerContainer();
        
        /* Client event handler bind */
        BigBro.MultiplayerApi.PeerConnected += new MultiplayerApi.PeerConnectedEventHandler(PeerConnectHandle);
        BigBro.MultiplayerApi.PeerDisconnected += new MultiplayerApi.PeerDisconnectedEventHandler(PeerDisConnectHandle);

        return true;
    }

    public static void PeerConnectHandle(long id)
    {
        var connectedClients = BigBro.MultiplayerApi.GetPeers();
        foreach (var connectedClient in connectedClients)
        {
            if (connectedClient == id)
            {
                if (PlayerContainer.GetNodeOrNull<Character>(id.ToString()) != null)
                {
                    LogTool.DebugLogDump("[" + id + "]Has connected");
                    return;
                }
                LogTool.DebugLogDump("Client[" + id + "]Connected!");
                PlayerAdd(id.ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse]);
                return;
            }
        }
        LogTool.DebugLogDump("[" + id + "]Don't connect");
    }

    public static void PeerDisConnectHandle(long id)
    {
        LogTool.DebugLogDump("Client[" + id + "]Disconnected!");
        PlayerRemove(id.ToString());
    }

    public static void CreatePlayerContainer()
    {
        var playerContainer = ResourceLoader.Load<PackedScene>(BigBro.PlayerContainerPath).InstantiateOrNull<PlayerContainer>();
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
        BigBro.PlayerContainer = playerContainer;
    }

    public static void PlayerAdd(string name, NodePath path)
    {
        PlayerContainer.PlayerInit(name);
        var character = ResourceLoader.Load<PackedScene>(path).Instantiate();
        if (character == null)
        {
            LogTool.DebugLogDump("Character Instantiate faild!");
            return;
        }
        character.Name = name;
        if (IsMultiplayer == false && character.Name == "1")
        {
            BigBro.Player = (Character)character;
        }
        BigBro.PlayerContainer.AddChild(character);
    }

    private static void PlayerRemove(string id)
    {
        var quittedClient = BigBro.PlayerContainer.GetNodeOrNull(id.ToString());
        if (quittedClient != null)
        {
            quittedClient.QueueFree();
            for (int i = 0; i < PlayerContainer.Players.Count; i++)
            {
                if (PlayerContainer.Players[i].Id == id)
                {
                    PlayerContainer.Players.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
