using Godot;
using System;

public partial class Bunny : Enemy
{
	private RayCast2D rayCast; // Checks if the edge of the floor is reached

	private bool playedDeath = false;
	private bool isTurning = false;
	private float direction = Vector2.Left.X;
	private bool onScreen = false;

	public override void _Ready()
	{
		base._Ready();
		rayCast = GetNode<RayCast2D>("RayCast2D");

		// Move in the direction towards the player when they enter the screen
		Camera cam = GameManager.instance.GetNode<Camera>("Camera2D");
		if (cam.Position.X < GlobalPosition.X) {
			direction = Vector2.Left.X;
			sprite.FlipH = false;
		}
		else {
			direction = Vector2.Right.X;
			sprite.FlipH = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (healthComponent.health > 0.0f && onScreen) {
			Vector2 velocity = Velocity;
			if (!IsOnFloor()) {
				velocity.Y += gravity * (float)delta;
			}

			if (IsOnFloor()) {
				velocity.X = direction * speed;
				animPlayer.Play("walk");
			}

			if (IsOnWall() || (IsOnFloor() && !rayCast.IsColliding())) {
				Turn();
			}

			Velocity = velocity;
			MoveAndSlide();
		}
		else if (healthComponent.health <= 0.0f) {
			if (!playedDeath) {
				playedDeath = true;

				animPlayer.Play("death");
				audio.playSFX("res://Sounds/SFX/explosion.wav", -15.0f);
			}

			if (!animPlayer.IsPlaying()) {
				QueueFree();
			}
		}
	}

	private async void Turn() {
		if (!isTurning) {
			isTurning = true;

			direction = -direction;
			sprite.FlipH = !sprite.FlipH;

			await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

			isTurning = false;
		}
	}

	private void ScreenEntered() {
		onScreen = true;
	}

	private void ScreenExited() {
		onScreen = false;
		QueueFree();
	}
}

