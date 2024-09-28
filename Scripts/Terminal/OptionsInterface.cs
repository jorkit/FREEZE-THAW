using Godot;
using System;

public partial class OptionsInterface : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var optionContainer = GetNode<OptionContainer>("OptionContainer");
        var textEdit_IpInput = new TextEdit();
        textEdit_IpInput.Position = optionContainer.GetNode<TouchScreenButton>("ClientJoinButton").Position + new Vector2(160, 20);
        textEdit_IpInput.Text = "192.168.1.68";
		textEdit_IpInput.Size = new Vector2(300, 35);
		textEdit_IpInput.Name = "ServerAddress";
        optionContainer.AddChild(textEdit_IpInput);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}