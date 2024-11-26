using FreezeThaw.Utils;
using Godot;

public partial class ProtoMatchMain : Node
{
    private Timer _timerGameOver;
    private Timer _timerMonsterEnter;
    public override void _EnterTree()
    {
        var playerContainer = PlayerControler.PlayerContainer;
        if (playerContainer == null)
        {
            LogTool.DebugLogDump("PlayerContainer not found!");
            return;
        }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _timerGameOver = new Timer();
        _timerGameOver.Timeout += TimerGameOverTimeOutHandler;
        BigBro.bigBro.AddChild(_timerGameOver);
        _timerGameOver.Start(30);

        _timerMonsterEnter = new Timer();
        _timerMonsterEnter.Timeout += TimerMonsterEnterTimeOutHandler;
        BigBro.bigBro.AddChild(_timerMonsterEnter);
        _timerMonsterEnter.Start(3);

        //var player = PlayerContainer.Players.Find(item=>item.Id == "1");
        //player.Hosting = true;
        //PlayerContainer.Players[0] = player;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void TimerGameOverTimeOutHandler()
    {

        PlayerContainer.SurviveScoreAdd();
        SceneFSM.PreStateChange(BigBro.SceneFSM, SceneStateEnum.MatchSettlement, true);
    }

    private void TimerMonsterEnterTimeOutHandler()
    {
        PlayerControler.MonsterEnter(PlayerContainer.Players[3].Id);
        _timerMonsterEnter.Stop();
    }
}
