using Godot;
using System;

public partial class Flower : CharacterBody2D
{
	private AnimationPlayer animPlayer;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool isRising = true;
	private bool gotAnimation = false;

	private Vector2 targetPosition;

	private HealthComponent healthComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("Rise");
		isRising = true;

		targetPosition = new Vector2(GetRandomXPosition(), -50.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Destroy when it hits the floor
		if (IsOnFloor() || healthComponent.health <= 0) {
			QueueFree();
		}

		if (isRising) {
			Vector2 direction = GlobalPosition.DirectionTo(targetPosition);
			velocity = direction * 400.0f;
		}
		else {
			velocity.X = 0.0f;
			velocity.Y = 1 * 50.0f;
			GetRandomFallAnimation();
		}		

		Velocity = velocity;
		MoveAndSlide();
	}

	private async void ScreenExited() {
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		isRising = false;
	}

	private float GetRandomXPosition() {
		Random rnd = new Random();
		int num = rnd.Next(642);
		return num; 
	}

	private void GetRandomFallAnimation() {
		if (!gotAnimation) {
			gotAnimation = true;
			Random rnd = new Random();
			int num = rnd.Next(2);

			if (num == 0) {
				animPlayer.Play("Fall1");
			}
			else if (num == 1) {
				animPlayer.Play("Fall2");
			}
			else {
				animPlayer.Play("Rise");
			}
		}
	}
}
