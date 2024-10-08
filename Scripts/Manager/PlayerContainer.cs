using FreezeThaw.Utils;
using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;

public partial class PlayerContainer : Node
{
	public struct Player
	{
		public string Id;
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
		if (BigBro.IsMultiplayer == true)
		{
            _timer = new();
            _timer.Timeout += TimerTimeOutHandler;
            GetParent().AddChild(_timer);
            _timer.Start(1);
		}
        else
        {
            _timer = new();
            _timer.Timeout += TimerTimeOutHandler;
            GetParent().AddChild(_timer);
            _timer.Start(1);
        }
	}

    public void TimerStop()
    {
        _timer.Stop();
    }

    public void PlayerInit(string id)
	{
        Player newPlayer = new()
        {
            Id = id,
            NickName = "",
            Score = SCORE_INIT,
            SurvivorPath = Character.CharacterPathList[Character.CharacterTypeEnum.Mouse],
            MonsterPath = Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm],
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
        if (BigBro.IsMultiplayer == true && BigBro.MultiplayerApi.IsServer() == false)
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
        var rpcRes = RpcId(BigBro.MultiplayerApi.GetRemoteSenderId(), "ResponseDataRpc", playersJson);
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
