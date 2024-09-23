using Godot;
using System;
using static Survivor;

public partial class FreezeThawButton : TouchScreenButton
{
	bool CanPress;
	Survivor survivor;
    Node survivors;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (Manager.Player?.GetType() == typeof(Monster))
        {
            CanPress = false;
            Pressed += PressedHandler;
        }
        else if (Manager.Player?.GetType() == typeof(Survivor))
        {
            CanPress = true;
            if (Manager.Player?.GetType() != typeof(Survivor))
            {
                Visible = false;
                return;
            }
            survivor = Manager.Player as Survivor;
            Pressed += PressedHandler;
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Manager.Player?.GetType() == typeof(Monster))
		{
            survivors = GetTree().Root.GetNodeOrNull("Main/Survivors");
            if (survivors == null)
            {
                GD.Print("Survivors nou found!");
                CanPress = false;
                return;
            }
            foreach (Node node in survivors.GetChildren())
            {
                if (((Survivor)node).GetNodeOrNull<FSM>("FSM").CurrentState.Name == "Freezed")
                {
                    CanPress = true;
                    return;
                }
                CanPress = false;
            }

        }
		else if (Manager.Player?.GetType() == typeof(Survivor))
		{
            if (survivor.CharacterState != FSMState.CharacterStateEnum.Hurt)
            {
                CanPress = true;
            }
            else
            {
                CanPress = false;
            }
        }
		
	}

	public void PressedHandler()
	{
        if (Manager.Player?.GetType() == typeof(Monster))
        {
            if (CanPress == true)
            {
                foreach (Node node in survivors.GetChildren())
                {
                    if (((Survivor)node).GetNodeOrNull<FSM>("FSM").CurrentState.Name == "Freezed")
                    {
                        GD.Print("Seal!");
                        FSMState.CharacterStateChange((Survivor)node, FSMState.CharacterStateEnum.Seal);
                    }
                }
            }
        }
        else if (Manager.Player?.GetType() == typeof(Survivor))
        {
            if (CanPress == true)
            {
                var fsm = survivor.GetNodeOrNull<FSM>("FSM");
                if (fsm != null)
                {
                    FSMState currentState = fsm.CurrentState;
                    switch (currentState.Name)
                    {
                        case "Idle":
                        case "Run":
                            survivor.CharacterState = FSMState.CharacterStateEnum.Freezing;
                            break;
                        case "Attack":
                        case "Armor":
                        case "Freezing":
                            break;
                        case "Freezed":
                            survivor.CharacterState = FSMState.CharacterStateEnum.Thaw;
                            break;
                        case "Seal":
                        case "Thaw":
                        case "Free":
                            break;
                    }
                }
            }
        } /* end of elseif */
	}
}
