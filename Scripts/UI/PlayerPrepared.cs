using FreezeThaw.Utils;
using Godot;
using System;
using System.Linq;

public partial class PlayerPrepared : TextureButton
{
	private bool Added {  get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == true && GetIndex() != 0)
		{
            Pressed += PressedHandler;
			Added = false;
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void PressedHandler()
    {
		if (Added == false)
		{
			/* find the unselect Survivor and move the button position to the Player position */
			for (int i = (int)Character.CharacterTypeEnum.SurvivorStart + 1; i < (int)Character.CharacterTypeEnum.SurvivorMax; i++)
			{
				if (PlayerContainer.Players.Find(item=>item.SurvivorNickName == Enum.GetName((Character.CharacterTypeEnum)i)).SurvivorNickName == Enum.GetName((Character.CharacterTypeEnum)i))
				{
					continue;
				}
                PlayerControler.PlayerAdd(i.ToString(), (Character.CharacterTypeEnum)i, true);
                GetParent().MoveChild(this, PlayerContainer.Players.Count - 1);
                Added = true;
                break;
            }
		}
		else
		{
			PlayerControler.PlayerRemove(PlayerContainer.Players[GetIndex()].Id);
            GetParent().MoveChild(this, PlayerContainer.Players.Count);
            Added = false;
		}
    }
}
