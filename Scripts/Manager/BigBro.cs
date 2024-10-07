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

    public static bool IsMultiplayer { set; get; }
    public static MultiplayerApi MultiplayerApi { set; get; }
    public static ENetMultiplayerPeer Peer { set; get; }
    public static MultiplayerSpawner Spawner { set; get; }
    
    public static Character Player { set; get; }
    public static Monster Monster { set; get; }
    public static PlayerContainer PlayerContainer { set; get; }
    public static readonly string PlayerContainerPath = "res://Scenes/Manager/PlayerContainer.tscn";
    public static Godot.Collections.Array<Survivor> Survivors { set; get; }
    
    public static SceneFSM SceneFSM { set; get; }

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
        SceneFSM.SetInitState();
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
                PlayerContainer.PlayerInit(id.ToString());
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
        var character = ResourceLoader.Load<PackedScene>(path).Instantiate();
        if (character == null)
        {
            LogTool.DebugLogDump("Character Instantiate faild!");
            return;
        }
        character.Name = name;
        if (character.GetType() == typeof(Sandworm))
            BigBro.Player = (Character)character;
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

    public static void PlayerTranslate(string id)
    {
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == false)
        {
            return;
        }
        for (int i = 0; i < BigBro.PlayerContainer.Players.Count; i++)
        {
            if (BigBro.PlayerContainer.Players[i].Id == id)
            {
                var player = BigBro.PlayerContainer.GetNodeOrNull<Survivor>(id);
                if (player == null)
                {
                    LogTool.DebugLogDump("Survivor instance not found!");
                    return;
                }
                var survivor = ResourceLoader.Load<PackedScene>(BigBro.PlayerContainer.Players[i].SurvivorPath).InstantiateOrNull<Survivor>();
                var monster = ResourceLoader.Load<PackedScene>(BigBro.PlayerContainer.Players[i].MonsterPath).InstantiateOrNull<Monster>();
                survivor.Name = BigBro.Monster.Name;
                survivor.Position = BigBro.Monster.Position;
                monster.Name = player.Name;
                monster.Position = player.Position;
                BigBro.Monster.Free();
                player.Free();
                BigBro.PlayerContainer.AddChild(survivor);
                BigBro.PlayerContainer.AddChild(monster);
                return;
            }
        }
        LogTool.DebugLogDump("Player [" + id + "] not found!");
    }
}
