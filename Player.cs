using Godot;
using System;

public partial class Player : Tank
{
	private AnimatedSprite2D TankBody;
	private Area2D Hitbox;

	public override void _Ready()
	{
		base._Ready();

		// Get references to player-specific nodes
		TankBody = GetNode<AnimatedSprite2D>("TankBody");
		
		// Connect Area2D signal for collisions
		var hitbox = GetNode<Area2D>("Hitbox");
		hitbox.BodyEntered += OnBodyEntered;
	}

	protected override void Control(double delta)
	{
		float deltaF = (float)delta;
		Vector2 direction = Vector2.Zero;

		// --- Tank rotation ---
		if (Input.IsActionPressed("turn_left"))
			Rotation -= RotationSpeed * deltaF;
		if (Input.IsActionPressed("turn_right"))
			Rotation += RotationSpeed * deltaF;

		// --- Movement ---
		if (Input.IsActionPressed("forward"))
			direction = Vector2.Up.Rotated(Rotation);
		else if (Input.IsActionPressed("backward"))
			direction = Vector2.Down.Rotated(Rotation);

		Velocity = direction * MoveSpeed;

		// --- Animate treads ---
		if (Velocity.Length() > 0f)
		{
			float forwardDot = Velocity.Normalized().Dot(Vector2.Up.Rotated(Rotation));
			TankBody.SpeedScale = forwardDot > 0f ? 1f : -1f;

			if (TankBody.Animation != "drive")
				TankBody.Animation = "drive";
			
			TankBody.Play();
		}
		else
		{
			TankBody.Stop();
		}

		// --- GunPivot rotation toward mouse ---
		if (GunPivot != null)
			GunPivot.LookAt(GetGlobalMousePosition());

		// --- Shooting ---
		if (Input.IsActionJustPressed("shoot") && CanShoot)
			Shoot();
	}
	
	// Handle direct enemy collision
	private void OnBodyEntered(Node body)
	{;
		if (body.IsInGroup("enemy"))
		{
			Die();
		}
	}

}
