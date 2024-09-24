using Godot;
using System;
using FreezeThaw.Utils;

public enum CharacterStateEnum
{
    Idle = 0,
    Run = 1,
    Attack = 2,
    Armor = 3,
    Hurt = 4,
    Freezing = 5,
    Freezed = 6,
    Thawing = 7,
    Sealing = 8,
    Sealed = 9,
    Unsealing = 10,
    Freeing = 11,
    Die = 12,
    MAX
};
public partial class FSM : Node
{
    private NodePath InitState;
    public FSMState CurrentState { set; get; }
    public CharacterStateEnum PreState { set; get; }
    public Character character;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        character = GetParent<Character>();
        if (character == null)
        {
            LogTool.DebugLogDump("Character not found!");
            return;
        }
        InitState = "Idle";
        SetInitState();
    }

    private void RemoveSelf()
    {
        GetParent().RemoveChild(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
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

    public static void PreStateChange(FSM fsm, CharacterStateEnum preState, bool force)
    {
        if (fsm == null)
        {
            LogTool.DebugLogDump("FSM not found!");
            return;
        }
        if (force == true)
        {
            fsm.PreState = preState;
            return;
        }
        switch (preState)
        {
            case CharacterStateEnum.Idle:
            case CharacterStateEnum.Run:
            case CharacterStateEnum.Attack:
            case CharacterStateEnum.Armor:
            case CharacterStateEnum.Hurt:
                if (preState > fsm.PreState)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Freezing:
                if (fsm.CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Freezed:
                if (fsm.CurrentState.StateIndex == CharacterStateEnum.Freeing)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Thawing:
                if (fsm.CurrentState.StateIndex == CharacterStateEnum.Freezed)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Sealing:
                if (fsm.CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Sealed:
                if (fsm.CurrentState.StateIndex == CharacterStateEnum.Freezed)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Unsealing:
                if (fsm.CurrentState.StateIndex == CharacterStateEnum.Sealed)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Freeing:
                if (fsm.CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    fsm.PreState = preState;
                }
                break;
            case CharacterStateEnum.Die:
                fsm.PreState = preState;
                break;
        }
    }

    private void SetInitState()
    {
        var validStates = GetChildren();
        if (validStates.Count <= 0)
        {
            LogTool.DebugLogDump("ValidStates not found!");
            return;
        }
        foreach (FSMState validState in validStates)
        {
            if (validState.StateIndex == CharacterStateEnum.Idle)
            {
                CurrentState = validState;
            }
        }
        if (CurrentState != null)
        {
            CurrentState.OnEnter();
        }
        else
        {
            LogTool.DebugLogDump("No init state found!");
        }
    }

    private void CheckStateTransition()
    {
        var count = GetChildCount();

        while (count > 0)
        {
            FSMState state = GetChild<FSMState>(--count);
            if (state != CurrentState)
            {
                if (state.EnterCondition())
                {
                    if (CurrentState.ExitCondition())
                    {
                        CurrentState.OnExit();
                        CurrentState = state;
                        CurrentState.OnEnter();
                    }
                    count = -1;
                }
            }
        }
    }
}
public abstract partial class FSMState : Node
{
    /*  -----------Run<=======-------->Frezzing---->Freezed----->Sealed---->Die
     * |            |         ||                       |            |
     * |            |         ||                       |            |
     * |     -----Attack<=====||                       |            |
     * |    |       |         L|                       |            |
     * |    |       |        Idle                      |            |
     * |    |       |         TT                       |            |
     * |    |     Armor<======||                       |            |
     * |    |       |         ||                       |            |
     * |    |       |         ||                       |            |     
     *  ---------->Hurt<======  ------Thawing<----------------------
     */
    public CharacterStateEnum StateIndex;
    protected FSM Fsm;

    protected FSMState()
    {
        
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Fsm = (FSM)GetParent();
        if (Fsm == null)
        {
            LogTool.DebugLogDump(Name + " FSM not found!");
            return;
        }

        for (CharacterStateEnum cse = 0; cse < CharacterStateEnum.MAX; cse++)
        {
            if (Name.ToString().EndsWith(cse.ToString()))
            {
                StateIndex = cse;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public abstract void Update(double delta);

    public abstract bool EnterCondition();
    public abstract void OnEnter();
    public abstract bool ExitCondition();
    public abstract void OnExit();
}
