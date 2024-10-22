using FreezeThaw.Utils;
using Godot;
using System;

public partial class CharacterItem : Sprite2D
{
    public Character.CharacterTypeEnum Type { get; set; }
    public Texture2D TextureImage { get; set; }
    private bool Draging { get; set; }
    private float DragStart { get; set; }

    private float ItemWide = 500;
  
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TextureImage = ResourceLoader.Load(Character.CharacterImagePathList[Type]) as Texture2D;
        if (TextureImage == null)
        {
            LogTool.DebugLogDump("TextureImage not found!");
            return;
        }
        Texture = TextureImage;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (((Node2D)GetParent()).Visible == false || (@event is not InputEventScreenTouch && @event is not InputEventScreenDrag))
        {
            return;
        }
        if (GetViewportRect().HasPoint((Vector2)@event.Get("position")) == false && Draging == false)
        {
            return;
        }
        /* if draging over limit, move to the other side */
        var limitLeft = -ItemWide * (Character.CharacterTypeEnum.SurvivorMax - 1 - Character.CharacterTypeEnum.SurvivorStart - 3);
        if (Position.X < limitLeft)
        {
            Position = new Vector2(ItemWide * 3, 0);
            DragStart -= (-limitLeft + ItemWide * 3);
        }
        var limitRight = ItemWide * (Character.CharacterTypeEnum.SurvivorMax - 1 - Character.CharacterTypeEnum.SurvivorStart - 1);
        if (Position.X > limitRight)
        {
            Position = new Vector2(-ItemWide, 0);
            DragStart += (limitRight + ItemWide);
        }
        /* record touch position */
        if (@event is InputEventScreenTouch && @event.IsPressed())
        {
            Draging = true;
            DragStart = ToLocal(((Vector2)@event.Get("position"))).X;
        }
        /* release touch */
        if (@event is InputEventScreenTouch && !@event.IsPressed())
        {
            var offset = Position.X % ItemWide;
            if (offset > 250)
            {
                Position += new Vector2(ItemWide - offset, 0);
            }
            else if (0 < offset )
            {
                Position -= new Vector2(offset, 0);
            }
            else if (offset < -250)
            {
                Position += new Vector2(-ItemWide - offset, 0);
            }
            else if (offset < 0)
            {
                Position -= new Vector2(offset, 0);
            }
            Draging = false;
            DragStart = 0;
        }
        /* roll list depends on Drag distance */
        if (@event is InputEventScreenDrag && Draging == true)
        {
            var newX = ToLocal((Vector2)@event.Get("position")).X;
            Position += new Vector2(newX - DragStart, 0);
        }
    }
}
