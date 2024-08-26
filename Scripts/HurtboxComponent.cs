using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	public bool takingDamage = false;
	private HealthComponent healthComponent;

	private bool isInvincible = false;
	private bool check = false;
	private bool checkOverlapping = false;	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetParent().GetNode<HealthComponent>("HealthComponent");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		isInvincible = healthComponent.isInvincible;

		if (GetParent().IsInGroup("Player")) {
			if (checkOverlapping)
				checkOverlappingHitboxes();
		}
	}

	private async void OnAreaEntered(Area2D area) {
		if (!takingDamage && area.IsInGroup("Hitbox") && !isInvincible) {
			takingDamage = true;

			if (IsInGroup("Player"))
				await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
			else if (IsInGroup("Enemy"))
				await ToSignal(GetTree().CreateTimer(0.35f), SceneTreeTimer.SignalName.Timeout);
			else if (IsInGroup("ImpactBoss"))
				await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);

			checkOverlapping = true;
			takingDamage = false;
		}
	}

	private void checkOverlappingHitboxes() {
		// If player is still standing in hitbox after
		// invincibility frames end then take damage
		if (!isInvincible && HasOverlappingAreas()) {
			if (!check) {
				check = true;

				// Check the overlapping areas
				Godot.Collections.Array<Area2D> array = GetOverlappingAreas();
				for (int i = 0; i < array.Count; i++) {
					if (!takingDamage && array[i].IsInGroup("Hitbox")) {
						// Damage player
						HitboxComponent hitbox = array[i].GetParent().GetNode<HitboxComponent>("HitboxComponent");
						hitbox.OnAreaEntered(this);

						// Change takingDamage value
						OnAreaEntered(array[i]);
						break;
					}
				}
				checkOverlapping = false;
				check = false;
			}
		}
	}
}
