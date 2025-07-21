using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	private HealthComponent healthComponent;
	private HitboxComponent hitbox;

	public bool tookDamage = false;
	public bool isInvincible = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetParent().GetNode<HealthComponent>("HealthComponent");
		hitbox = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
	{
		if (tookDamage) {
			if (!isInvincible) {
				isInvincible = true;

				if (GetParent().IsInGroup("Player"))
					await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
				else if (GetParent().IsInGroup("Boss"))
					await ToSignal(GetTree().CreateTimer(1.8f), SceneTreeTimer.SignalName.Timeout);
				else if (GetParent().IsInGroup("ImpactBoss"))
					await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

				tookDamage = false;

				if (GetParent().IsInGroup("Player"))
					await ToSignal(GetTree().CreateTimer(1.2f), SceneTreeTimer.SignalName.Timeout);

				isInvincible = false;
			}
		}
		else if (GetParent().IsInGroup("Player") && !tookDamage && HasOverlappingAreas() && hitbox != null && !isInvincible) {
			// Damage player if are colliding with a hitbox after the invincibility ends
			CallDamage();
		}
	}

	private void OnAreaEntered(Area2D area) {
		if (area.IsInGroup("Hitbox")) {
			hitbox = (HitboxComponent)area;
			
			if (!isInvincible) {
				CallDamage();
			}
		}
	}

	private void OnAreaExited(Area2D area) {
		if (!HasOverlappingAreas()) {
			hitbox = null;
		}
	}
	
	// Calls the Damage() function in HealthComponent
	private void CallDamage() {
		tookDamage = true;
		HealthComponent healthComponent = GetParent().GetNode<HealthComponent>("HealthComponent");
		healthComponent.Damage(hitbox.GetDamageNumber(), hitbox.GetParent());
	}	
}
