using FreezeThaw.Utils;
using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class PlayerContainer : Node
{
	public struct Player
	{
		public string Id;
		public string NickName;
		public int Score;
	}
    public List<Player> Players { set; get; }
	private int SCORE_INIT { set; get; }

	private static Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SCORE_INIT = 300;
		Players = new List<Player>();
        ChildEnteredTree += new ChildEnteredTreeEventHandler(ChildEnterTreeHandler);
		if (BigBro.IsMultiplayer == true)
		{
            if (BigBro.MultiplayerApi.IsServer() == false)
            {
                _timer = new();
                _timer.Timeout += TimerTimeOutHandler;
                GetParent().AddChild(_timer);
                _timer.Start(1);
            }
		}
        else
        {
            _timer = new();
            _timer.Timeout += TimerTimeOutHandler;
            GetParent().AddChild(_timer);
            _timer.Start(1);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ChildEnterTreeHandler(Node node)
	{

        if (BigBro.IsMultiplayer == true)
		{
            Player newPlayer = new()
            {
                Id = node.Name,
                NickName = node.GetType().ToString(),
                Score = SCORE_INIT
            };
            Players.Add(newPlayer);
        }
		else
		{
            Player newPlayer = new()
            {
                Id = node.Name,
                NickName = node.GetType().ToString(),
                Score = SCORE_INIT
            };
            Players.Add(newPlayer);
        }
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
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == false)
            {
                var rpcRes = RpcId(MultiplayerPeer.TargetPeerServer, "RequestDataRpc");
                if (rpcRes != Error.Ok)
                {
                    LogTool.DebugLogDump("RequestDataRpc failed! " + rpcRes.ToString());
                }
            }
        }
        else
        {
            var players = GetChildren();
            if (players.Count <= 0)
            {
                LogTool.DebugLogDump("Players not found!");
                return;
            }
            for (int i = 0; i < players.Count; i++)
            {
                var label = players[i].GetNodeOrNull<Label>("ScoreLabel");
                if (label == null)
                {
                    LogTool.DebugLogDump("Label not found!");
                    continue;
                }
                label.Text = Players[i].Score.ToString();
                LogTool.DebugLogDump(label.Text);
            }
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
		var players = GetChildren();
		if (players.Count <= 0)
		{
			LogTool.DebugLogDump("Players not found!");
			return;
		}
		for (int i = 0; i < players.Count; i++)
		{
			var label = players[i].GetNodeOrNull<Label>("ScoreLabel");
			if (label == null)
			{
				LogTool.DebugLogDump("Label not found!");
				continue;
			}
            label.Text = Players[i].Score.ToString();
		}
    }
}
