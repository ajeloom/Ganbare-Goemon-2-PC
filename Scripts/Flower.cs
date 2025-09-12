using Godot;
using System;

public partial class Flower : CharacterBody2D
{
	private AnimationPlayer animPlayer;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private float flyUpSpeed = 400.0f;

	private bool isRising = true;
	private bool gotAnimation = false;
	private bool isDestroyed = false;

	private Vector2 targetPosition;

	private HealthComponent healthComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("Rise");
		isRising = true;

		targetPosition = new Vector2(GetRandomXPosition(), GlobalPosition.Y - 100.0f - GetRandomYPosition());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Destroy when it hits the floor
		if (IsOnFloor() || !animPlayer.IsPlaying()) {
			QueueFree();
		}

		if (healthComponent.health <= 0) {
			if (!isDestroyed) {
				isDestroyed = true;
				velocity.Y = 0.0f;
				animPlayer.Play("Explosion");
			}
		}
		else {
			if (isRising) {
				GlobalPosition = GlobalPosition.MoveToward(targetPosition, (float)delta * flyUpSpeed);
				if (GlobalPosition == targetPosition) {
					isRising = false;
				}
			}
			else {
				velocity.X = 0.0f;
				velocity.Y = 1 * 50.0f;
				GetRandomFallAnimation();
			}	
		}	

		Velocity = velocity;
		MoveAndSlide();
	}

	private float GetRandomXPosition() {
		Random rnd = new Random();
		int num = rnd.Next(128);

		int negative = rnd.Next(2);
		if (negative < 1) {
			num = -num;
		}

		return num; 
	}
	
	private float GetRandomYPosition() {
		Random rnd = new Random();
		int num = rnd.Next(45) + 1;

		return num; 
	}

	private void GetRandomFallAnimation() {
		if (!gotAnimation) {
			gotAnimation = true;
			Random rnd = new Random();
			int num = rnd.Next(3);

			if (num == 0) {
				animPlayer.Play("Fall1");
			}
			else if (num == 1) {
				animPlayer.Play("Fall2");
			}
			else if (num == 2) {
				animPlayer.Play("Fall3");
			}
		}
	}
}
