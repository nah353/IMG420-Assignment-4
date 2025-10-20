using Godot;
using System;

public partial class StationaryEnemy : Tank
{
	private Node2D PlayerTarget; // Player reference

	public override void _Ready()
	{
		base._Ready();

		// Find the Player node
		PlayerTarget = GetTree().Root.GetNodeOrNull<Node2D>("../Player")
			?? GetTree().GetFirstNodeInGroup("player") as Node2D;
	}

	protected override void Control(double delta)
	{
		if (PlayerTarget == null || GunPivot == null)
			return;

		// Aim gun toward player
		GunPivot.LookAt(PlayerTarget.GlobalPosition);
		
		if(CanShoot)
		{
			Shoot();
		}
	}
}
