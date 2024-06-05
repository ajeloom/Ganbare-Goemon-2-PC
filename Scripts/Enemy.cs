using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public bool takingDamage = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private AnimationPlayer animPlayer;
	[Export] private HealthComponent healthComponent;
	[Export] private HitboxComponent hitboxComponent;
	[Export] private AudioStreamPlayer2D audio;
	private CollisionShape2D hitbox;

	public override void _Ready()
	{
		hitbox = (CollisionShape2D)hitboxComponent.GetChild(0);
		hitbox.Disabled = false;
		animPlayer.Play("Idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (healthComponent.health <= 0.0f) {
			Explosion();
		}
		else {
			animPlayer.Play("Idle");

			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
				velocity.Y += gravity * (float)delta;

			// Handle Jump.
			if (IsOnFloor())
				velocity.Y = JumpVelocity;

			Velocity = velocity;
			MoveAndSlide();
		}
	}

	private async void Explosion() {
		hitbox.Disabled = true;
		animPlayer.Play("Explosion");
		audio.Play();
		await ToSignal(GetTree().CreateTimer(0.35f), SceneTreeTimer.SignalName.Timeout);
		this.QueueFree();
	}
}
