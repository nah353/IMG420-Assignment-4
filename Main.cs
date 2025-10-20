using Godot;
using System;

public partial class Main : Node
{
	// PackedScene references for levels
	[Export] public PackedScene Level1Scene;
	[Export] public PackedScene Level2Scene;
	[Export] public PackedScene Level3Scene;

	private Node currentLevel;

	public override void _Ready()
	{
		// Get the TitleScreen node (assumes it's a direct child of Main)
		var titleScreen = GetNode<CanvasLayer>("TitleScreen");

		// Get the buttons from TitleScreen
		var level1Button = titleScreen.GetNode<Button>("ButtonContainer/Level1");
		var level2Button = titleScreen.GetNode<Button>("ButtonContainer/Level2");
		var level3Button = titleScreen.GetNode<Button>("ButtonContainer/Level3");

		// Connect button signals to load levels
		level1Button.Pressed += () => LoadLevel(Level1Scene);
		level2Button.Pressed += () => LoadLevel(Level2Scene);
		level3Button.Pressed += () => LoadLevel(Level3Scene);
	}

	private void LoadLevel(PackedScene levelScene)
	{
		// Remove existing level if it exists
		if (currentLevel != null)
		{
			currentLevel.QueueFree();
			currentLevel = null;
		}

		// Instance the new level
		currentLevel = levelScene.Instantiate();
		AddChild(currentLevel);

		// Optionally hide the TitleScreen
		var titleScreen = GetNode<CanvasLayer>("TitleScreen");
		titleScreen.Visible = false;
	}
}
