using Godot;
using System;

public partial class PatrollingEnemy : Tank
{
	[Export] public float PathSpeed = 0.1f; // How fast to move along the path
	private PathFollow2D pathFollow;
	private Node2D player;
	private AnimatedSprite2D TankBody;

	public override void _Ready()
	{
		base._Ready();

		// Get PathFollow2D parent
		pathFollow = GetParent<PathFollow2D>();
		
		TankBody = GetNodeOrNull<AnimatedSprite2D>("TankBody");

		// Find the player
		player = GetTree().Root.GetNodeOrNull<Node2D>("../Player")
				 ?? GetTree().GetFirstNodeInGroup("player") as Node2D;
	}

	protected override void Control(double delta)
	{
		if (pathFollow == null)
			return;

		// Move along the path
		pathFollow.ProgressRatio += PathSpeed * (float)delta;
		
		TankBody.Play();

		// Aim gun toward player
		GunPivot.LookAt(player.GlobalPosition);
		
		if(CanShoot)
		{
			Shoot();
		}
	}
}
