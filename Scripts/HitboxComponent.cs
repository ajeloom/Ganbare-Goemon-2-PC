using Godot;
using System;

public partial class HitboxComponent : Area2D
{
	[Export] private float damageNumber = 2.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
	}

	public float GetDamageNumber() {
		return damageNumber;
	}
}
