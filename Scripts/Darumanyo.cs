using Godot;
using System;

public partial class Darumanyo : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private AnimationPlayer animPlayer;
	[Export] private HealthComponent healthComponent;
	[Export] private HitboxComponent hitboxComponent;
	[Export] private AudioComponent audio;
	private CollisionShape2D hitbox;

	private bool isExploding = false;

	public override void _Ready()
	{
		hitbox = (CollisionShape2D)hitboxComponent.GetChild(0);
		hitbox.Disabled = false;
		animPlayer.Play("Idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (healthComponent.health > 0.0f) {
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
		else {
			if (!isExploding) {
				isExploding = true;
				hitbox.Disabled = true;
				animPlayer.Play("Explosion");
				audio.playSFX("res://Sounds/SFX/explosion.wav", -15.0f);
			}

			if (!animPlayer.IsPlaying()) {
				QueueFree();
			}
		}
	}
	
	private void OnScreenExited() {
		QueueFree();
	}
}
