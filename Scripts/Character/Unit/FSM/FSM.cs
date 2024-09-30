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
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == false)
            { 
                return;
            }
            var rpcResult = Rpc("PreStateChangeRPC", (int)newPreState, force);
            if (rpcResult != Error.Ok)
            {
                LogTool.DebugLogDump("Rpc error: " + rpcResult.ToString());
            }
        }
        else
        {
            PreStateChangeHandle(newPreState, force);
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void PreStateChangeRPC(int newPreState_Int, bool force)
    {
        PreStateChangeHandle((CharacterStateEnum)newPreState_Int, force);
    }

    private void PreStateChangeHandle(CharacterStateEnum newPreState, bool force)
    {
        if (force == true && CurrentState.StateIndex != CharacterStateEnum.Die)
        {
            LogTool.DebugLogDump("Force Change To " + newPreState.ToString());
            PreState = newPreState;
            return;
        }
        LogTool.DebugLogDump("Change To " + newPreState.ToString());
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
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == true)
            {
                var count = GetChildCount();
                while (count > 0)
                {
                    FSMState state = GetChild<FSMState>(--count);
                    if (state == CurrentState)
                    {
                        continue;
                    }
                    if (state.EnterCondition() == false)
                    {
                        continue;
                    }
                    if (CurrentState.ExitCondition() == false)
                    {
                        continue;
                    }
                    var rpcRes = Rpc("CurrentStateChange", (int)state.StateIndex);
                    if (rpcRes != Error.Ok)
                    {
                        LogTool.DebugLogDump("Rpc call failed! " + rpcRes.ToString());
                        return;
                    }
                    count = -1;
                }
            }
        }
        else
        {
            var count = GetChildCount();
            while (count > 0)
            {
                FSMState state = GetChild<FSMState>(--count);
                if (state == CurrentState)
                {
                    continue;
                }
                if (state.EnterCondition() == false)
                {
                    continue;
                }
                if (CurrentState.ExitCondition() == false)
                {
                    continue;
                }
                CurrentState.OnExit();
                CurrentState = state;
                CurrentState.OnEnter();
            }
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void CurrentStateChange(int stateIndex)
    {
        CurrentState.OnExit();
        CurrentState = GetNode<FSMState>(((CharacterStateEnum)stateIndex).ToString());
        CurrentState.OnEnter();
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
            if (Name.ToString() == cse.ToString())
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
