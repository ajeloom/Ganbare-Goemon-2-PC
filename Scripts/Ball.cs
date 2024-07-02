using Godot;
using System;

public partial class Ball : CharacterBody2D
{
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private HitboxComponent hitboxComponent;
	private AnimationPlayer animPlayer;
	private ScreenShake camera;
	private Sprite2D sprite;

	private bool isExploding = false;
	private bool changedColor = false;

	private Vector2 targetPosition;
	private Vector2 velocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<ScreenShake>("/root/Impact Battle/Camera2D");
		sprite = GetNode<Sprite2D>("Sprite2D");
		Scale = new Vector2(0.245f, 0.245f);

		targetPosition = new Vector2(GetRandomXPosition(), 60.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!changedColor) {
			changedColor = true;
			RandomColor();
		}

		if (healthComponent.health > 0.0f) {
			// Scale the ball based on how far or close they are (w/ y-position)
			if (Position.Y < 0.0f)
				Scale = new Vector2(0.245f, 0.245f);
			else
				Scale = new Vector2(0.035f * Position.Y, 0.035f * Position.Y);

			// Hurtbox can be hit when ball gets close
			if (Scale.X < 1.0f) {
				hurtboxComponent.Monitorable = false;
			}
			else {
				hurtboxComponent.Monitorable = true;
			}

			// Adjust the speed the ball moves based on the target position
			var direction = GlobalPosition.DirectionTo(targetPosition);
			float speed = Math.Abs(targetPosition.X - Position.X);

			speed *= 2.0f;
			if (speed < 30.0f) {
				velocity = direction * 30.0f;
			}
			else {
				velocity = direction * speed;
			}
			
			// Damage the player when it gets too close
			if (Position.Y >= 60.0f) {
				HealthComponent hp = GetNode<HealthComponent>("/root/Impact Battle/Impact/HealthComponent");
				hp.Damage(10, hitboxComponent);

				camera.ApplyShake(5.0f);

				var scene = GD.Load<PackedScene>("res://Scenes/Crack.tscn");
				Node2D instance = scene.Instantiate<Node2D>();
				instance.GlobalPosition = new Vector2(Position.X, Position.Y - 100.0f);
				AddSibling(instance, true);

				healthComponent.health = 0.0f;
			}

			animPlayer.Play("Idle");

			Velocity = velocity;
			MoveAndSlide();
		}
		else {
			if (!isExploding) {
				isExploding = true;
				animPlayer.Play("Explosion");
			}
			
			if (!animPlayer.IsPlaying())
				QueueFree();
		}
		
	}

	private void RandomColor() {
		Random rnd = new Random();
		int num = rnd.Next(4);
		if (num == 0 || num == 1 || num == 2 || num == 3) {
			sprite.Frame = num;
		}
	}

	private float GetRandomXPosition() {
		Random rnd = new Random();
		int num = rnd.Next(205);

		int negative = rnd.Next(2);
		if (negative == 0) {
			num = -num;
		}
		return num; 
	}
}
