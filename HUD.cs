using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private Label LivesRemainingLabel;
	private Label Message;
	private Tank Player;
	private Timer StartTimer;
	private Timer EndTimer;

	private bool gameEnded = false;

	public override void _Ready()
	{
		// Get UI elements
		LivesRemainingLabel = GetNode<Label>("TankTexture/LivesRemaining");
		Message = GetNode<Label>("Message");
		StartTimer = GetNode<Timer>("StartTimer");
		EndTimer = GetNode<Timer>("EndTimer");

		// Connect timer signals
		StartTimer.Timeout += OnStartTimerTimeout;
		EndTimer.Timeout += OnEndTimerTimeout;

		// Pause at the start and show countdown message
		GetTree().Paused = true;
		StartTimer.Start();

		// Get the Player node
		Player = GetNodeOrNull<Tank>("../Player");
	}

	public override void _Process(double delta)
	{
		if (gameEnded)
			return;

		// Update LivesRemaining text
		if (Player != null && LivesRemainingLabel != null)
			LivesRemainingLabel.Text = Player.Health.ToString();

		// Update "Go!" message
		if (StartTimer.TimeLeft <= 1.0 && Message.Visible)
			Message.Text = "Go!";

		// --- Check game end conditions ---
		if (GetTree().GetNodesInGroup("enemy").Count == 0)
		{
			EndGame("Level Complete!");
		}
		else if (GetTree().GetNodesInGroup("player").Count == 0)
		{
			EndGame("Game Over!");
		}
	}

	private void OnStartTimerTimeout()
	{
		// Hide message and unpause the game
		Message.Visible = false;
		GetTree().Paused = false;
	}

	private void EndGame(string endText)
	{
		gameEnded = true;

		Message.Text = endText;
		Message.Visible = true;

		EndTimer.Start();
	}

	private void OnEndTimerTimeout()
	{
		// Return to main level select menu
		GetTree().ReloadCurrentScene();
	}
}
