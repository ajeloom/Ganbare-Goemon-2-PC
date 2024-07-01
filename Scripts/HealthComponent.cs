using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] public float health = 12.0f;
	[Export] private AnimationPlayer animPlayer;
	[Export] private AnimationPlayer effectsPlayer;
	private AudioComponent audioComponent;

	public bool isInvincible = false;
	public bool isHitting = false;
	public bool takingDamage = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioComponent = GetNode<AudioComponent>("AudioComponent");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Damage(float damageNumber, Node damageDealer) {
		if (!isInvincible) {
			health -= damageNumber;
			takingDamage = true;
			if (damageDealer.GetParent().IsInGroup("Player")) {
				audioComponent.playSFX("res://Sounds/SFX/Goemon/hit.wav", -12.5f);
			}
		}

		// Blink red when hurt
		if (health > 0.0f) {
			if (IsInGroup("Player"))
				InvincibilityFrames(1.2f);
			else if (IsInGroup("ImpactBoss"))
				InvincibilityFrames(0.5f);
			else if (IsInGroup("Boss"))
				InvincibilityFrames(1.8f);
		}
	}

	private async void InvincibilityFrames(float time) {
		if (!isInvincible) {
			isInvincible = true;
			
			// Wait for the hurt animation to finish
			if (IsInGroup("Player"))
				await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);

			effectsPlayer.Play("Hurt");
			await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
			effectsPlayer.Play("RESET");

			takingDamage = false;
			isInvincible = false;
		}
	}
}
