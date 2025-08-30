using Godot;
using System;

public partial class Bunny : CharacterBody2D
{
	[Export] public float speed = 65.0f;
	[Export] public float jumpVelocity = -400.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private AudioComponent audio;
	private Sprite2D sprite;
	private RayCast2D rayCast; // Checks if the edge of the floor is reached

	private bool playedDeath = false;
	private bool isTurning = false;
	private float direction = Vector2.Left.X;
	private bool onScreen = false;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		audio = GetNode<AudioComponent>("AudioComponent");
		sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.FlipH = false;
		rayCast = GetNode<RayCast2D>("RayCast2D");
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

