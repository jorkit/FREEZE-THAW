using Godot;
using System;
using FreezeThaw.Utils;
using static Godot.WebSocketPeer;

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
    private CharacterStateEnum InitState;
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
        InitState = CharacterStateEnum.Idle;
        SetInitState();
    }

    private void SetInitState()
    {
        var count = GetChildCount();

        while (count > 0)
        {
            FSMState state = GetChild<FSMState>(--count);
            if (state.StateIndex == InitState)
            {
                CurrentState = state;
                CurrentState.OnEnter();
                return;
            }
        }
        LogTool.DebugLogDump("No init state found!");
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
            /* Monster can also operate during getting hurt, just add some Animation*/
            if (character.GetType().BaseType == typeof(Monster) && ((Monster)character).Hurting == true)
            {
                var states = GetChildren();
                foreach (FSMState state in states)
                {
                    if (state.StateIndex == CharacterStateEnum.Hurt)
                    {
                        state.Update(delta);
                    }
                }
            }
            CheckStateTransition();
        }
        else
        {
            LogTool.DebugLogDump("No cur state found!");
        }
    }

    /* Prestate change, wait for CurrentState change */
    public void PreStateChange(CharacterStateEnum newPreState, bool force)
    {
        Rpc("PreStateChangeRPC", (int)newPreState, force); 
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void PreStateChangeRPC(int newPreState_Int, bool force)
    {
        CharacterStateEnum newPreState = (CharacterStateEnum)newPreState_Int;
        if (force == true && CurrentState.StateIndex != CharacterStateEnum.Die)
        {
            LogTool.DebugLogDump("Force Change To " + newPreState.ToString());
            PreState = newPreState;
            return;
        }
        switch (newPreState)
        {
            case CharacterStateEnum.Idle:
            case CharacterStateEnum.Run:
            case CharacterStateEnum.Attack:
            case CharacterStateEnum.Armor:
            case CharacterStateEnum.Hurt:
                if (newPreState > PreState)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Freezing:
                if (CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Freezed:
                if (CurrentState.StateIndex == CharacterStateEnum.Freeing)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Thawing:
                if (CurrentState.StateIndex == CharacterStateEnum.Freezed)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Sealing:
                if (CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Sealed:
                if (CurrentState.StateIndex == CharacterStateEnum.Freezed)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Unsealing:
                if (CurrentState.StateIndex == CharacterStateEnum.Sealed)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Freeing:
                if (CurrentState.StateIndex < CharacterStateEnum.Attack)
                {
                    PreState = newPreState;
                }
                break;
            case CharacterStateEnum.Die:
                PreState = newPreState;
                break;
        }
    }

    /// <summary>
    /// Check condition and transition ready state
    /// </summary>
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
                        count = -1;
                    }
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
