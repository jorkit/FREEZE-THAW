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
		public string NickName;
		public int Score;
        public string SurvivorPath;
        public string MonsterPath;
    }
    public List<Player> Players { set; get; }
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
        string SurvivorPath;
        string MonsterPath;
        if (NetworkControler.IsMultiplayer == true)
        {
            /* if player exist */
            var player = Players.Find(character => character.Id == id);
            if (player.Id == id)
            {
                return;
            }
            SurvivorPath = Character.CharacterPathList[Character.CharacterTypeEnum.Mouse];
            MonsterPath = Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm];
        }
        else
        {
            SurvivorPath = Character.CharacterPathList[Character.CharacterTypeEnum.Mouse];
            MonsterPath = Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm];
        }
        Player newPlayer = new()
        {
            Id = id,
            NickName = "",
            Score = SCORE_INIT,
            SurvivorPath = SurvivorPath,
            MonsterPath = MonsterPath,
        };
        Players.Add(newPlayer);
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
            ScoreLabelUpdate();
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
        ScoreLabelUpdate();
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
