using FreezeThaw.Utils;
using Godot;
using System;

public partial class CharacterItem : Sprite2D
{
    public Character.CharacterTypeEnum Type { get; set; }
    public Texture2D TextureImage { get; set; }
    private bool Draging { get; set; }
    private float DragStart { get; set; }
  
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TextureImage = ResourceLoader.Load(Character.CharacterImagePathList[Type]) as Texture2D;
        Texture = TextureImage;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (((Node2D)GetParent()).Visible == false)
        {
            return;
        }
        if (GetViewportRect().HasPoint((Vector2)@event.Get("position")) == false && Draging == false)
        {
            return;
        }
        if (Position.X < -500)
        {
            Position = new Vector2(1500, 0);
            DragStart -= 2000;
        }
        if (Position.X > 1500)
        {
            Position = new Vector2(-500, 0);
            DragStart += 2000;
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
            var offset = Position.X % 500;
            if (offset > 250)
            {
                Position += new Vector2(500 - offset, 0);
            }
            else if (0 < offset )
            {
                Position -= new Vector2(offset, 0);
            }
            else if (offset < -250)
            {
                Position += new Vector2(-500 - offset, 0);
            }
            else if (offset < 0)
            {
                Position -= new Vector2(offset, 0);
            }
            Draging = false;
            DragStart = 0;
        }
        /* move list depends on Drag distance */
        if (@event is InputEventScreenDrag && Draging == true)
        {
            var newX = ToLocal((Vector2)@event.Get("position")).X;
            Position += new Vector2(newX - DragStart, 0);
        }
    }
}
