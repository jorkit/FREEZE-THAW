using Godot;
using System;

public partial class FSM : Node
{
	private NodePath InitState { set; get; }
    public FSMState CurrentState { set; get; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParentOrNull<Character>().GetType() != typeof(Survivor))
        {
            FSMState state = (FSMState)GetNodeOrNull("Freezing");
            if (state != null)
            {
                RemoveChild(state);
                state.QueueFree();
            }
            state = (FSMState)GetNodeOrNull("Freezed");
            if (state != null)
            {
                RemoveChild(state);
                state.QueueFree();
            }
            state = (FSMState)GetNodeOrNull("Thaw");
            if (state != null)
            {
                RemoveChild(state);
            }
            state = (FSMState)GetNodeOrNull("Seal");
            if (state != null)
            {
                RemoveChild(state);
            }
            state = (FSMState)GetNodeOrNull("Free");
            if (state != null)
            {
                RemoveChild(state);
            }
            //CallDeferred("queue_free");
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
			GD.Print("No cur state found!");
		}
    }

    private void SetInitState()
	{
        CurrentState = GetNodeOrNull<FSMState>(InitState);
		if (CurrentState != null)
		{
			CurrentState.OnEnter();
		}
		else
		{
			GD.Print("No init state found!");
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
    /*  -----------Run<=======-------->Frezzing---->Freezed----->Sealed
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
    public enum CharacterStateEnum
    {
        Idle = 0,
        Run = 1,
        Attack = 2,
        Armor = 3,
        Hurt = 4,
        Freezing = 5,
        Freezed = 6,
        Seal = 7,
        Thaw = 8,
        Free = 9
    };
    public Character character;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        character = GetParent().GetParentOrNull<Character>();
        if (character == null)
        {
            GD.Print("Character not found!");
        }
        GD.Print(character);
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

	public static void CharacterStateChange(Character characterToChange, CharacterStateEnum newPreState)
	{
        Type type = characterToChange.GetType();
        if (type == typeof(Monster))
        {
            switch (newPreState)
            {
                case CharacterStateEnum.Idle:
                    ((Monster)characterToChange).CharacterState = newPreState;
                    break;
                case CharacterStateEnum.Run:
                case CharacterStateEnum.Attack:
                case CharacterStateEnum.Armor:
                case CharacterStateEnum.Hurt:
                    if (newPreState > ((Monster)characterToChange).CharacterState)
                    {
                        ((Monster)characterToChange).CharacterState = newPreState;
                    }
                    break;
                default:
                    GD.Print("Invalid State for Monster!");
                    break;
            }
        }
        if (type == typeof(Survivor))
        {
            var curPreState = ((Survivor)characterToChange).CharacterState;
            switch (newPreState)
            {
                /* lowest original state */
                case CharacterStateEnum.Idle:
                    if (curPreState >= CharacterStateEnum.Hurt && curPreState != CharacterStateEnum.Thaw)
                        break;
                    ((Survivor)characterToChange).CharacterState = newPreState;
                    break;
                /* interrupt by order */
                case CharacterStateEnum.Run:
                case CharacterStateEnum.Attack:
                case CharacterStateEnum.Armor:
                case CharacterStateEnum.Hurt:
                    if (newPreState > curPreState)
                    {
                        ((Survivor)characterToChange).CharacterState = newPreState;
                    }
                    break;
                /* Non interruptible */
                case CharacterStateEnum.Freezing:
                    if (curPreState > (newPreState - 1))
                        break;
                    ((Survivor)characterToChange).CharacterState = newPreState;
                    break;
                case CharacterStateEnum.Freezed:
                case CharacterStateEnum.Seal:
                    if (curPreState != (newPreState - 1))
                        break;
                    ((Survivor)characterToChange).CharacterState = newPreState;
                    break;
                case CharacterStateEnum.Thaw:
                    if (curPreState == (newPreState - 1))
                        ((Survivor)characterToChange).CharacterState = CharacterStateEnum.Free;
                    else if (curPreState == (newPreState - 2))
                        ((Survivor)characterToChange).CharacterState = newPreState;
                    break;
            }
        }
    }
}
