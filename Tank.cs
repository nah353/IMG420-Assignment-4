using Godot;
using System;

public partial class Tank : CharacterBody2D
{
	[Export] public PackedScene BulletScene { get; set; }
	[Export] public PackedScene ExplosionScene { get; set; }
	[Export] public float MoveSpeed { get; set; } = 200f;
	[Export] public float RotationSpeed { get; set; } = 3f;
	[Export] public float GunCooldown { get; set; } = 1f;
	[Export] public int Health { get; set; } = 1;

	public bool CanShoot { get; private set; } = true;
	public bool IsAlive { get; private set; } = true;

	protected Timer GunCooldownTimer;
	protected Node2D GunPivot;
	protected Node2D Muzzle;
	protected AudioStreamPlayer2D ShootSound;

	public override void _Ready()
	{
		CallDeferred(nameof(InitNodes));
	}

	private void InitNodes()
	{
		GunPivot = GetNodeOrNull<Node2D>("GunPivot");
		if (GunPivot != null)
			Muzzle = GunPivot.GetNodeOrNull<Node2D>("Gun/Muzzle");

		GunCooldownTimer = GetNodeOrNull<Timer>("GunCooldownTimer");
		if (GunCooldownTimer != null)
		{
			GunCooldownTimer.WaitTime = GunCooldown;
			GunCooldownTimer.Timeout += OnGunCooldownTimeout;
		}
		
		ShootSound = GetNode<AudioStreamPlayer2D>("ShootSound");

		// Delay shooting readiness by one frame to ensure timers are connected
		Callable.From(() => CanShoot = true).CallDeferred();
	}

	protected virtual void Control(double delta)
	{
		// Overridden in Player or StationaryEnemy
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsAlive)
			return;

		Control(delta);
		MoveAndSlide();
	}

	protected void Shoot()
	{
		if (BulletScene == null || !CanShoot || Muzzle == null)
		{
			return;
		}
		
		ShootSound.Play();

		CanShoot = false;
		GunCooldownTimer?.Start();

		var bullet = BulletScene.Instantiate<Bullet>();
		GetTree().Root.AddChild(bullet);

		bullet.Start(Muzzle.GlobalPosition, GunPivot.GlobalRotation);
	}

	private void OnGunCooldownTimeout()
	{
		CanShoot = true;
	}

	public void Hit()
	{
		if (!IsAlive)
			return;

		Health--;
		if (Health <= 0)
		{
			IsAlive = false;
			Die();
		}
	}

	protected void Die()
	{	
		if (ExplosionScene != null)
		{
			var explosion = ExplosionScene.Instantiate<GpuParticles2D>();
			GetTree().Root.AddChild(explosion);
			explosion.GlobalPosition = GlobalPosition;
			explosion.Emitting = true;
			explosion.Restart();
			
			var dieSound = explosion.GetNodeOrNull<AudioStreamPlayer2D>("DieSound");
			dieSound?.Play();
		}

		QueueFree();
	}
}
