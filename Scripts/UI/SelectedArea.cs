using FreezeThaw.Utils;
using Godot;
using System;

public partial class SelectedArea : Node2D
{
	private static Sprite2D SurvivorSelectedImage {  get; set; }
	private static Sprite2D MonsterSelectedImage { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SurvivorSelectedImage = GetNodeOrNull<Sprite2D>("SurvivorMask/SurvivorSelectedImage");
		if (SurvivorSelectedImage == null)
		{
			LogTool.DebugLogDump("SurvivorSelectedImage not found!");
			return;
		}
        MonsterSelectedImage = GetNodeOrNull<Sprite2D>("MonsterMask/MonsterSelectedImage");
		if (MonsterSelectedImage == null)
		{
            LogTool.DebugLogDump("MonsterSelectedImage not found!");
            return;
        }
		Position = new Vector2(UIControler.WindowSize.X / 4, UIControler.WindowSize.Y / 2);

        var texture = ResourceLoader.Load("res://Static/Animations/Character/Survivors/Mouse/MouseSelected.png") as Texture2D;
        SurvivorSelectedImage.Texture = texture;
        texture = ResourceLoader.Load("res://Static/Animations/Character/Monsters/Sandworm/Adonis_boss.png") as Texture2D;
        MonsterSelectedImage.Texture = texture;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void SetSelectedImage(Texture2D texture, bool isSurvivor)
	{
		if (isSurvivor)
		{
            SurvivorSelectedImage.Texture = texture;
        }
		else
		{
            MonsterSelectedImage.Texture = texture;
        }
    }
}
