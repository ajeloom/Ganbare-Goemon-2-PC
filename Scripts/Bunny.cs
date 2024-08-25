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

	private bool playedDeath = false;
	private float lastXPos;
	private bool gotDisplacement = false;
	private float direction = -1.0f;
	private bool onScreen = false;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		audio = GetNode<AudioComponent>("AudioComponent");
		sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.FlipH = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (healthComponent.health > 0.0f) {
			if (onScreen) {
				Vector2 velocity = Velocity;
				if (!IsOnFloor())
					velocity.Y += gravity * (float)delta;

				if (IsOnFloor()) {
					velocity.X = direction * speed;
					animPlayer.Play("walk");
				}

				if (!gotDisplacement) {
					gotDisplacement = true;
					getLastPosX();
					gotDisplacement = false;
				}

				Velocity = velocity;
				MoveAndSlide();
			}
		}
		else {
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

	private async void getLastPosX() {
		lastXPos = Position.X;
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
		if (lastXPos - Position.X == 0 && onScreen) {
			direction = -direction;
			sprite.FlipH = (sprite.FlipH) ? false : true;
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

