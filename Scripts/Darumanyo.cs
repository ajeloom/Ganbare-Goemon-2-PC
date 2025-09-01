using Godot;
using System;

public partial class Darumanyo : Enemy
{
	private CollisionShape2D hitbox;

	private bool isExploding = false;

	public override void _Ready()
	{
		base._Ready();
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
				velocity.Y = jumpVelocity;

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
