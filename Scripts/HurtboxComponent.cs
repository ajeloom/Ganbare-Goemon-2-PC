using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	public bool takingDamage = false;
	private HealthComponent healthComponent;

	private bool isInvincible = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetParent().GetNode<HealthComponent>("HealthComponent");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		isInvincible = healthComponent.isInvincible;
	}

	private async void OnAreaEntered(Area2D area) {
		if (!takingDamage && area.IsInGroup("Hitbox") && !isInvincible) {
			takingDamage = true;

			if (IsInGroup("Player"))
				await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
			else if (IsInGroup("Enemy"))
				await ToSignal(GetTree().CreateTimer(0.35f), SceneTreeTimer.SignalName.Timeout);
			takingDamage = false;
		}

		if (area.IsInGroup("Goal")) {
			GetTree().Quit();
		}
	}
}
