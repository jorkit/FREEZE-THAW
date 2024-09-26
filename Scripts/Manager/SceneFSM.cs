using FreezeThaw.Utils;
using Godot;
using System;

public enum SceneStateEnum
{
    GameOpenLoading = 0,
	Login = 1,
	OptionsInterface = 2,
    MatchStartLoading = 3,
	MatchMain = 4,
    MatchSettlement = 5,
    MAX
};
public partial class SceneFSM : Node
{
    private SceneStateEnum InitState;
    public SceneFSMState CurrentState { set; get; }
    public SceneStateEnum PreState { set; get; }

    public BigBro bigbro;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        bigbro = GetParent<BigBro>();
        if (bigbro == null)
        {
            LogTool.DebugLogDump("BigBro not found!");
            return;
        }
        InitState = SceneStateEnum.GameOpenLoading;
        PreState = SceneStateEnum.GameOpenLoading;
    }

    public void SetInitState()
    {
        var count = GetChildCount();
        while (count > 0)
        {
            SceneFSMState state = GetChild<SceneFSMState>(--count);
            if (state.StateIndex == InitState)
            {
                CurrentState = state;
                CurrentState.OnEnter();
                return;
            }
        }
        LogTool.DebugLogDump("No init state found!");
    }

    /* Prestate change, wait for CurrentState change */
    public static void PreStateChange(SceneFSM sceneFsm, SceneStateEnum newPreState, bool force)
    {
        if (sceneFsm == null)
        {
            LogTool.DebugLogDump("FSM not found!");
            return;
        }
        if (force == true)
        {
            LogTool.DebugLogDump("Force Change To " + newPreState.ToString());
            sceneFsm.PreState = newPreState;
            return;
        }
        switch (newPreState)
        {
            case SceneStateEnum.GameOpenLoading:
                break;
            case SceneStateEnum.Login:
            case SceneStateEnum.MatchStartLoading:
            case SceneStateEnum.MatchMain:
            case SceneStateEnum.MatchSettlement:
                if (newPreState == sceneFsm.CurrentState.StateIndex + 1)
                {
                    sceneFsm.PreState = newPreState;
                }
                break;
            case SceneStateEnum.OptionsInterface:
                if (sceneFsm.CurrentState.StateIndex == SceneStateEnum.Login || sceneFsm.CurrentState.StateIndex == SceneStateEnum.MatchSettlement)
                {
                    sceneFsm.PreState = newPreState;
                }
                break;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if (CurrentState != null)
        {
            CurrentState.Update(delta);
            CheckStateTransition();
        }
        else
        {
            LogTool.DebugLogDump("No cur state found!");
        }
    }

    private void CheckStateTransition()
    {
        var count = GetChildCount();

        while (count > 0)
        {
            SceneFSMState state = GetChild<SceneFSMState>(--count);
            if (state != CurrentState)
            {
                if (state.EnterCondition())
                {
                    if (CurrentState.ExitCondition())
                    {
                        CurrentState.OnExit();
                        CurrentState = state;
                        CurrentState.OnEnter();
                        count = -1;
                    }
                }
            }
        }
    }
}

public abstract partial class SceneFSMState : Node
{
    public SceneStateEnum StateIndex;
    protected SceneFSM SceneFsm;
    protected string Path;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SceneFsm = GetParent<SceneFSM>();
        if (SceneFsm == null)
        {
            LogTool.DebugLogDump(Name + " SceneFsm not found!");
            return;
        }
        for (SceneStateEnum cse = 0; cse < SceneStateEnum.MAX; cse++)
        {
            if (Name.ToString().EndsWith(cse.ToString()))
            {
                StateIndex = cse;
            }
        }
    }

    public abstract void Update(double delta);
    public abstract bool EnterCondition();
    public abstract void OnEnter();
    public abstract bool ExitCondition();
    public abstract void OnExit();
}
