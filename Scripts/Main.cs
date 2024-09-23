using Godot;
using System;

public partial class Main : Node2D
{
	public Monster monster;
    public Godot.Collections.Array<Survivor> survivors;
    public CharacterBody2D Player;
	Vector2I screenSize;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CallDeferred("Add");
    }
    public void Add()
    {
        GetTree().Root.GetNode<Main>("Main").AddChild(ResourceLoader.Load<PackedScene>("res://Scenes/Characters/Survivors/Survivor.tscn").InstantiateOrNull<Survivor>());
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
