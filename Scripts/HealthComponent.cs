using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] public float health = 12.0f;
	[Export] private AnimationPlayer animPlayer;
	[Export] private AnimationPlayer effectsPlayer;
	private AudioComponent audioComponent;

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
		health -= damageNumber;
		
		if (GetParent().IsInGroup("Player")) {
			Player player = GetParent<Player>();
			PlayerUI ui = player.GetNode<PlayerUI>("Player UI");
			ui.UpdateHealth();
		}
		
		if (damageDealer.IsInGroup("Player")) {
			audioComponent.playSFX("res://Sounds/SFX/Goemon/hit.wav", -15.0f);
		}

		// Blink red when hurt
		if (health > 0.0f) {
			if (GetParent().IsInGroup("Player"))
				PlayDamageFlashInvincibilityAnimation(1.2f);
			else if (GetParent().IsInGroup("Boss"))
				PlayDamageFlashInvincibilityAnimation(1.8f);
			else if (GetParent().IsInGroup("ImpactBoss"))
				PlayDamageFlashInvincibilityAnimation(0.5f);
		}
	}

	private async void PlayDamageFlashInvincibilityAnimation(float time) {
		// Play flash animation after player hurt animation finishes playing
		if (GetParent().IsInGroup("Player"))
			await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);

		// Wait for the hurt animation to finish		
		effectsPlayer.Play("Hurt");
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
		effectsPlayer.Play("RESET");
	}
}
