using FreezeThaw.Utils;
using Godot;
using Godot.Collections;
using static PlayerContainer;

public partial class PlayerControler : Node
{
    public static Character Player { set; get; }
    public static Monster Monster { set; get; }
    public static PlayerContainer PlayerContainer { set; get; }
    public static readonly string PlayerContainerPath = "res://Scenes/Manager/PlayerContainer.tscn";
    public static Array<Survivor> Survivors { set; get; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (CreatePlayerContainer() == false)
        {
            LogTool.DebugLogDump("PlayerContainer create failed!");
            return;
        }
        AddChild(PlayerContainer);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static bool CreatePlayerContainer()
    {
        var playerContainer = ResourceLoader.Load<PackedScene>(PlayerContainerPath).InstantiateOrNull<PlayerContainer>();
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return false;
        }
        PlayerContainer = playerContainer;

        return true;
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
        if (NetworkControler.IsMultiplayer == false && character.Name == "1")
        {
            Player = (Character)character;
        }
        PlayerContainer.AddChild(character);
    }

    public static void PlayerRemove(string id)
    {
        var quittedClient = PlayerContainer.GetNodeOrNull(id.ToString());
        if (quittedClient != null)
        {
            quittedClient.Free();
            /*
            for (int i = 0; i < PlayerContainer.Players.Count; i++)
            {
                if (PlayerContainer.Players[i].Id == id)
                {
                    PlayerContainer.Players.RemoveAt(i);
                    break;
                }
            }
            */
        }
    }

    public static void PlayerTranslate(string id)
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == false)
        {
            return;
        }
        Survivor player = null;
        Survivor survivor = null;
        Monster monster = null;
        for (int i = 0; i < PlayerContainer.Players.Count; i++)
        {
            if (PlayerContainer.Players[i].Id == Monster.Name)
            {
                survivor = ResourceLoader.Load<PackedScene>(PlayerContainer.Players[i].SurvivorPath).InstantiateOrNull<Survivor>();
                survivor.Name = Monster.Name;
                survivor.Position = Monster.Position;
                if (NetworkControler.IsMultiplayer == false && survivor.Name == "1")
                    Player = survivor;
            }
            else if (PlayerContainer.Players[i].Id == id)
            {
                player = PlayerContainer.GetNodeOrNull<Survivor>(id);
                if (player == null)
                {
                    LogTool.DebugLogDump("Survivor instance not found!");
                    return;
                }
                monster = ResourceLoader.Load<PackedScene>(PlayerContainer.Players[i].MonsterPath).InstantiateOrNull<Monster>();
                monster.Name = player.Name;
                monster.Position = player.Position;
                if (NetworkControler.IsMultiplayer == false && monster.Name == "1")
                    PlayerControler.Player = monster;
            }
        }
        Monster?.Free();
        player?.Free();
        PlayerContainer.AddChild(survivor);
        PlayerContainer.AddChild(monster);
    }

    public static void PlayerTranslateToAI(string id)
    {
        var quittedClient = PlayerContainer.GetNodeOrNull(id.ToString());
        if (quittedClient == null)
        {
            LogTool.DebugLogDump("QuittedClient not found!");
            return;
        }
        string AIPath;
        if (quittedClient.GetType().BaseType == typeof(Monster))
        {
            AIPath = PlayerContainer.Players.Find(character => character.Id == id.ToString()).AIMonsterPath;
        }
        else
        {
            AIPath = PlayerContainer.Players.Find(character => character.Id == id.ToString()).AISurvivorPath;
        }
        PlayerRemove(id.ToString());
        PlayerAdd(id.ToString(), AIPath);
    }
}
