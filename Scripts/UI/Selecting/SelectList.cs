using FreezeThaw.Utils;
using Godot;
using System;
using System.Linq;

public partial class SelectList : Node2D
{
	public int ListStart {  get; set; }
    public int ListEnd { get; set; }

	private bool Draging = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (Name == "SurvivorList")
		{
			ListStart = (int)Character.CharacterTypeEnum.SurvivorStart;
			ListEnd = (int)Character.CharacterTypeEnum.SurvivorMax;
		}
		else
		{
            ListStart = (int)Character.CharacterTypeEnum.MonsterStart;
			ListEnd = (int)Character.CharacterTypeEnum.MonsterMax;
		}
		Visible = false;
		for (int i = ListStart + 1; i < ListEnd; i++)
		{
			var characterItem = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Selecting/CharacterItem.tscn").InstantiateOrNull<CharacterItem>();
			if (characterItem == null)
			{
				LogTool.DebugLogDump("CharacterItem instantiate faild!");
				return;
			}
			characterItem.Name = Enum.GetName((Character.CharacterTypeEnum)i);
			characterItem.Position = Vector2.Right * 500 * (i - ListStart - 1);
			characterItem.Type = (Character.CharacterTypeEnum)i;
			AddChild(characterItem);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		if (Visible == true && @event is InputEventScreenTouch && @event.IsPressed())
		{
            if (!Draging)
			{
				Draging = true;
				return;
			}
			var position = ToLocal((Vector2)@event.Get("position"));
            if (position.X > 250 && position.X < 750)
			{
                Select();
                WaitingHall.SelectingArea.MouseFilter = Control.MouseFilterEnum.Ignore;
            }
		}
		else if (@event is InputEventScreenDrag)
		{
            Draging = false;
		}
    }

	private void Select()
	{
        Visible = false;
        Draging = false;
        foreach (CharacterItem item in GetChildren())
        {
            if (item.Position.X == 500)
            {
				/* test */
                if (ListStart != (int)Character.CharacterTypeEnum.SurvivorStart)
                {
					SelectedArea.SetSelectedImage(item.TextureImage, true);
					PlayerControler.PlayerContainer.CharacterSelect(GetMultiplayerAuthority().ToString(), item.Type, true);
                }
				else
				{
                    SelectedArea.SetSelectedImage(item.TextureImage, false);
                    PlayerControler.PlayerContainer.CharacterSelect(GetMultiplayerAuthority().ToString(), item.Type, false);
                }
            }
        }
    }
}
