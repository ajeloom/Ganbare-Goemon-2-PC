using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] public float health = 12.0f;
	[Export] private AnimationPlayer animPlayer;
	[Export] private AnimationPlayer effectsPlayer;
	[Export] private AudioComponent audioComponent;

	public bool isInvincible = false;
	public bool isHitting = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print("HealthComponent is hitting: " + isHitting);
	}

	public void Damage(float damageNumber, Node damageDealer) {
		if (!isInvincible) {
			health -= damageNumber;
			if (damageDealer.GetParent().IsInGroup("Player")) {
				audioComponent.playSFX("res://Sounds/SFX/Goemon/hit.wav", -12.5f);
			}
		}

		// if (health <= 0 && IsInGroup("Enemy")) {
		// 	GetParent().QueueFree();
		// }

		// Blink red when hurt
		if (IsInGroup("Player"))
			InvincibilityFrames(1.2f);
		else if (IsInGroup("Boss"))
			InvincibilityFrames(1.8f);
		

		// if (health <= 0 && IsInGroup("Player")) {
		// 	GetTree().Quit();
		// }
	}

	private async void InvincibilityFrames(float time) {
		if (!isInvincible) {
			isInvincible = true;

			// Wait for the hurt animation to finish
			if (IsInGroup("Player"))
				await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);

			// Change hurtbox scale in order for the player to get hurt 
			// if still inside a hitbox when they are no longer invinicible
			HurtboxComponent hurtbox = GetParent().GetNode<HurtboxComponent>("HurtboxComponent");
			hurtbox.Scale = Vector2.Zero;

			effectsPlayer.Play("Hurt");
			await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
			effectsPlayer.Play("RESET");

			hurtbox.Scale = new Vector2(1.0f, 1.0f);
			isInvincible = false;
		}
	}
}
