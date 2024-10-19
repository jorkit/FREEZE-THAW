using FreezeThaw.Utils;
using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class PlayerContainer : Node
{
	public struct Player
	{
		public string Id;
        public bool Hosting;
        public bool Ready;
		public string NickName;
		public int Score;
        public Character.CharacterTypeEnum SurvivorType;
        public Character.CharacterTypeEnum MonsterType;
    }
    public static List<Player> Players { set; get; }
	public int SCORE_INIT { set; get; }

	private static Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SCORE_INIT = 300;
		Players = new List<Player>();
        //ChildEnteredTree += new ChildEnteredTreeEventHandler(ChildEnterTreeHandler);
        _timer = new();
        _timer.Timeout += TimerTimeOutHandler;
        GetParent().AddChild(_timer);
        TimerStart();
	}

    public override void _PhysicsProcess(double delta)
    {
        var players = GetChildren();
        foreach (var item in players)
        {
            if (item.GetType().BaseType == typeof(Survivor))
            {
                if (((Survivor)item).GetCurrentState() != CharacterStateEnum.Sealed)
                {
                    return;
                }
            }
        }
        /* all survivor sealed, reset state */
        foreach (var item in players)
        {
            if (item.GetType().BaseType == typeof(Survivor))
            {
                ((Survivor)item).Fsm.PreStateChange(CharacterStateEnum.Idle, true);
            }
        }
    }

    public static void TimerStart()
    {
        _timer.Start(1);
    }

    public static void TimerStop()
    {
        _timer.Stop();
    }

    public void PlayerInit(string id)
	{
        Character.CharacterTypeEnum survivorType;
        Character.CharacterTypeEnum monsterType;
        if (NetworkControler.IsMultiplayer == true)
        {
            /* if player exist */
            var player = Players.Find(character => character.Id == id);
            if (player.Id == id)
            {
                return;
            }
            survivorType = Character.CharacterTypeEnum.Mouse;
            monsterType = Character.CharacterTypeEnum.Sandworm;
        }
        else
        {
            survivorType = Character.CharacterTypeEnum.Mouse;
            monsterType = Character.CharacterTypeEnum.Sandworm;
        }
        LogTool.DebugLogDump(id);
        Player newPlayer = new()
        {
            Id = id,
            NickName = "",
            Score = SCORE_INIT,
            SurvivorType = survivorType,
            MonsterType = monsterType,
        };
        Players.Add(newPlayer);
    }

    public void CharacterSelect(string id, Character.CharacterTypeEnum characterType, bool isSurvivor)
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer())
        {
            CharacterSelectRpc(id, characterType, isSurvivor);
        }
        else
        {
            RpcId(MultiplayerPeer.TargetPeerServer, "CharacterSelectRpc", id, (int)characterType, isSurvivor);
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void CharacterSelectRpc(string id, Character.CharacterTypeEnum characterType, bool isSurvivor)
    {
        var player = Players.Find(item=>item.Id == id);
        if (player.Id != id)
        {
            LogTool.DebugLogDump("Player not found!");
            return;
        }
        if (isSurvivor)
        {
            player.SurvivorType = characterType;
        }
        else
        {
            player.MonsterType = characterType;
        }
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Id == id)
            {
                Players[i] = player;
                return;
            }
        }
    }

    public void PlayerReady(string id, bool ready)
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer())
        {
            PlayerReadyRpc(id, ready);
            NetworkControler.ReadyStatus = ready;
        }
        else
        {
            RpcId(MultiplayerPeer.TargetPeerServer, "PlayerReadyRpc", id, ready);
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void PlayerReadyRpc(string id, bool ready)
    {
        var player = Players.Find(item => item.Id == id);
        if (player.Id != id)
        {
            LogTool.DebugLogDump("Player not found!");
            return;
        }
        player.Ready = ready;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Id == id)
            {
                var readyButton = BigBro.bigBro.GetNodeOrNull<ReadyButton>("WaitingHall/PreparedArea/ReadyButton");
                if (readyButton == null)
                {
                    LogTool.DebugLogDump("ReadyButton not found!");
                    return;
                }
                readyButton.ReadyFireUpdate(ready);
                Players[i] = player;
                return;
            }
        }
        LogTool.DebugLogDump("Player not found!");
    }

    public void ChangeScore(string id, int score)
	{
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Id == id)
            {
				Player player = Players[i];
                player.Score += score;
				Players[i] = player;
            }
        }
    }

	private void TimerTimeOutHandler()
	{
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == false)
        {
            var rpcRes = RpcId(MultiplayerPeer.TargetPeerServer, "RequestDataRpc");
            if (rpcRes != Error.Ok)
            {
                LogTool.DebugLogDump("RequestDataRpc failed! " + rpcRes.ToString());
            }
        }
        else
        {
            //ScoreLabelUpdate();
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void RequestDataRpc()
	{
        var playersJson = JsonConvert.SerializeObject(Players);
        var rpcRes = RpcId(NetworkControler.MultiplayerApi.GetRemoteSenderId(), "ResponseDataRpc", playersJson);
        if (rpcRes != Error.Ok)
        {
            LogTool.DebugLogDump("ResponseDataRpc failed! " + rpcRes.ToString());
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void ResponseDataRpc(string playersJson)
    {
        Players = JsonConvert.DeserializeObject<List<Player>>(playersJson);
        LogTool.DebugLogDump(GetMultiplayerAuthority().ToString());
        NetworkControler.ReadyStatus = Players.Find(item => item.Id == GetMultiplayerAuthority().ToString()).Ready;
        LogTool.DebugLogDump(NetworkControler.ReadyStatus.ToString());
        //ScoreLabelUpdate();
    }

    private void ScoreLabelUpdate()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            var player = GetNodeOrNull<Character>(Players[i].Id);
            if (player == null)
            {
                LogTool.DebugLogDump("Player[" + Players[i].Id + "]not found!");
                return;
            }
            var label = player.GetNodeOrNull<Label>("ScoreLabel");
            if (label == null)
            {
                LogTool.DebugLogDump("Label not found!");
                continue;
            }
            label.Text = Players[i].Score.ToString();
        }
    }
}
