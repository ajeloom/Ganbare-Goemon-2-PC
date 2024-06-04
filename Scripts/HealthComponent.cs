using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] private float health = 12.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Damage(float damageNumber) {
		health -= damageNumber;

		// Delete player when health reaches 0
		if (health <= 0) {
			CharacterBody2D body = this.GetParent<CharacterBody2D>();
			body.QueueFree();
		}
	}
}
