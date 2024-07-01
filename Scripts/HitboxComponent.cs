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


	public void OnAreaEntered(Area2D area) {
		if (area.IsInGroup("Player") || area.IsInGroup("Enemy") || area.IsInGroup("ImpactBoss")) {
			CharacterBody2D body = area.GetParent<CharacterBody2D>();

			// Get the health component from the player
			HealthComponent hpComponent = body.GetNode<HealthComponent>("HealthComponent");
			hpComponent.Damage(damageNumber, this);
		}
	}
}
