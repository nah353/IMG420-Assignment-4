using Godot;
using System;

public partial class ChasingEnemy : Tank
{
	private Node2D player;
	private NavigationAgent2D agent;
	private float timeSinceLastUpdate = 0f;
	private AnimatedSprite2D TankBody;

	public override void _Ready()
	{
		base._Ready();

		// Get the AnimatedSprite2D for driving animation
		TankBody = GetNodeOrNull<AnimatedSprite2D>("TankBody");

		// Find the player
		player = GetTree().Root.GetNodeOrNull<Node2D>("../Player")
				 ?? GetTree().GetFirstNodeInGroup("player") as Node2D;

		if (player == null)
		{
			GD.PrintErr("Player not found!");
			return;
		}

		// Get the NavigationAgent2D node
		agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		if (agent == null)
		{
			GD.PrintErr("NavigationAgent2D node not found!");
			return;
		}

		agent.TargetPosition = player.GlobalPosition;
		agent.Radius = 8f; // Optional: collision avoidance radius
	}

	protected override void Control(double delta)
	{
		if (player == null || agent == null)
			return;

		// Aim gun toward player
		GunPivot.LookAt(player.GlobalPosition);

		// Update the agent's target
		agent.TargetPosition = player.GlobalPosition;

		// Get the next point from the navigation path
		Vector2 nextPoint = agent.GetNextPathPosition();
		Vector2 direction = (nextPoint - GlobalPosition).Normalized();

		// Set velocity using Tank's MoveSpeed
		Velocity = direction * MoveSpeed;

		// Rotate smoothly toward movement direction
		if (direction.LengthSquared() > 0.01f)
		{
			float targetAngle = direction.Angle();
			Rotation = Rotation + Mathf.Wrap(targetAngle - Rotation, -Mathf.Pi, Mathf.Pi) * (float)delta * RotationSpeed;
		}

		// Animate driving treads
		if (TankBody != null)
		{
			if (Velocity.Length() > 0f)
			{
				TankBody.Animation = "drive";
				TankBody.Play();
			}
			else
			{
				TankBody.Stop();
			}
		}
	}
}
