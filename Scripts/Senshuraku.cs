using Godot;
using System;

public partial class Senshuraku : CharacterBody2D
{
	[Export] public float speed = 100.0f;
	
	private Vector2 backPosition;
	private Vector2 frontPosition;
	private Vector2 targetPosition;

	private bool goToPos = false;

	// Attack variables
	private bool doingAttack = false;
	private bool playingBallAnim = false;
	private bool playingSpinAnim = false;
	[Export] private bool doingBallAttack = false;
	[Export] private bool doingSpinAttack = false;
	private bool moveRight = false;
	private bool spawnedBalls = false;

	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private HitboxComponent hitboxComponent;
	private Sprite2D sprite;
	private ScreenShake camera;

	private bool takingDamage = false;
	private bool isDead = false;

	private Vector2 velocity;
	private float gradualSpeed = 0.0f;

	public override void _Ready() {
		Position = new Vector2(0.0f, 10.0f);
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		sprite = GetNode<Sprite2D>("Sprite2D");
		camera = GetNode<ScreenShake>("/root/Impact Battle/Camera2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		velocity = Velocity;

		backPosition = new Vector2(Position.X, 7.0f);
		frontPosition = new Vector2(Position.X, 80.0f);

		if (healthComponent.health <= 0.0f) {
			if (!isDead) {
				isDead = true;
				animPlayer.Play("Death");

				if (!animPlayer.IsPlaying())
					QueueFree();
			}
		}
		else {
			// Scale the boss based on how far or close they are (w/ y-position)
			if (Position.Y < 0.0f)
				Scale = new Vector2(0.7f, 0.7f);
			else
				Scale = new Vector2(0.1f * Position.Y, 0.1f * Position.Y);

			// Limit the boss x position
			if (Position.X < -300.0f) {
				Position = new Vector2(-300.0f, Position.Y);
			}
			else if (Position.X > 300.0f) {
				Position = new Vector2(300.0f, Position.Y);
			}

			// Limit the boss y position
			if (Position.Y < 7.0f) {
				Position = new Vector2(Position.X, 7.0f);
			}
			else if (Position.Y > 80.0f) {
				Position = new Vector2(Position.X, 80.0f);
			}

			// Hurtbox can be hit when boss gets close
			if (Scale.X < 6.5f) {
				hurtboxComponent.Monitorable = false;
			}
			else {
				hurtboxComponent.Monitorable = true;
			}

			// Check if the boss is taking damage
			takingDamage = healthComponent.takingDamage;

			if (takingDamage) {
				animPlayer.Play("Hurt");
				doingBallAttack = false;
				doingSpinAttack = false;
				gradualSpeed = 0.0f;
				sprite.Rotation = 0.0f;
				var direction = GlobalPosition.DirectionTo(backPosition);
				velocity = direction * 200.0f;
				if (Position == backPosition) {
					camera.ApplyShake(2.0f);
				}
			}
			else {
				if (doingSpinAttack) {
					SpinAttack();
				}
				else {
					gradualSpeed = 0.0f;
					var direction = GlobalPosition.DirectionTo(backPosition);
					velocity = direction * 250.0f;
				}
				
				if (doingBallAttack) {
					ShootBallAttack();
				}
				else {
					
					if (!doingSpinAttack)
						velocity = Vector2.Zero;
				}

				if (!doingBallAttack && !doingSpinAttack) {
					playingSpinAnim = false;
					playingBallAnim = false;
					GetRandomAttack();
				}
			}								

			Velocity = velocity;
			MoveAndSlide();
		}	
	}

	private async void ShootBallAttack() {
		if (!playingBallAnim) {
			playingBallAnim = true;
			animPlayer.Play("BallAttack");
		}

		if (!moveRight) {
			// Go to the left
			targetPosition = new Vector2(-117.0f, 7.0f);

			if (Position.X <= -117.0f) {
				moveRight = true;
			}
		}
		else {
			// Go to the right
			targetPosition = new Vector2(117.0f, 7.0f);

			if (Position.X >= 117.0f) {
				moveRight = false;
				doingBallAttack = false;
				spawnedBalls = false;
			}

			if (!spawnedBalls) {
				spawnedBalls = true;
				for (int i = 0; i < 4; i++) {
					LoadScene("res://Scenes/Ball.tscn", Position.X, 7.0f);
					await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
				}
			}
		}
		
		var direction = GlobalPosition.DirectionTo(targetPosition);
		velocity = direction * speed;
	}

	private void SpinAttack() {
		// Go to the middle of the screen before starting attack
		if (Position.X >= 5.0f || Position.X <= -5.0) {
			var direction = GlobalPosition.DirectionTo(new Vector2(0.0f, 7.0f));			
			velocity = direction * 100.0f;
		}
		else {
			if (!playingSpinAnim) {
				playingSpinAnim = true;
				animPlayer.Play("SpinAttack");
			}

			var direction = GlobalPosition.DirectionTo(frontPosition);			
			velocity = direction * gradualSpeed;

			if (gradualSpeed < 50.0f) {
				gradualSpeed += 2.0f;
			}
			else if (gradualSpeed >= 50.0f) {
				gradualSpeed = 50.0f;
			}
		}

		// Boss lands the spin attack hit
		if (Position == frontPosition) {
			animPlayer.Pause();
			sprite.Rotation = 0.0f;
			camera.ApplyShake(20.0f);
			HealthComponent hp = GetNode<HealthComponent>("/root/Impact Battle/Impact/HealthComponent");
			hp.Damage(20, hitboxComponent);

			LoadScene("res://Scenes/Crack.tscn", Position.X, Position.Y - 80.0f);

			hurtboxComponent.Monitorable = false;
			doingSpinAttack = false;
		}
	}

	private void LoadScene(string name, float xPos, float yPos) {
		var scene = GD.Load<PackedScene>(name);
		Node2D instance = scene.Instantiate<Node2D>();
		instance.GlobalPosition = new Vector2(xPos, yPos);
		AddSibling(instance, true);
	}

	private void GetRandomAttack() {
		Random rnd = new Random();
		int num = rnd.Next(2);
		

		if (num == 0) {
			doingBallAttack = true;
			doingSpinAttack = false;
		}
		else if (num == 1) {
			doingBallAttack = false;
			doingSpinAttack = true;
		}
	}
}
