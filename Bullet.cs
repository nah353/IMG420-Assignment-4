using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
	[Export] public int Speed = 600;
	private int _bounceCount = 0;      // Tracks number of bounces
	[Export] private int MaxBounces = 1;       // Destroy after first bounce
	[Export] public PackedScene SmokeScene;
	private AudioStreamPlayer2D ImpactSound;

	public void Start(Vector2 position, float direction)
	{
		Rotation = direction;
		Position = position;
		Velocity = new Vector2(Speed, 0).Rotated(Rotation);
		
		// Spawn smoke at bullet start
		if (SmokeScene != null)
		{
			var smoke = SmokeScene.Instantiate<GpuParticles2D>();
			GetTree().Root.AddChild(smoke);     // add independently
			smoke.GlobalPosition = Position;     // place at bullet spawn
			smoke.Emitting = true;
			smoke.Restart();                     // trigger one-shot
		}
		
		ImpactSound = GetNode<AudioStreamPlayer2D>("Impact1");
	}

	public override void _PhysicsProcess(double delta)
	{
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			if (collision.GetCollider().HasMethod("Hit"))
			{
				collision.GetCollider().Call("Hit");
				QueueFree();
			}
			
			// Handle max bullet bounces
			_bounceCount++;
			if (_bounceCount > MaxBounces || (collision.GetCollider() is Node collider && collider.IsInGroup("bullet")))
			{
				QueueFree();
				return;
			}

			Velocity = Velocity.Bounce(collision.GetNormal());
			ImpactSound.Play();
		}

		// Rotate the bullet to match its current velocity
		if (Velocity.Length() > 0f)
			Rotation = Velocity.Angle();
	}

	private void OnVisibilityNotifier2DScreenExited()
	{
		QueueFree();
	}
}
