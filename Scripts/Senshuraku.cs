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
	private bool shootBallAttack = false;
	private bool playingAnim = false;
	private bool doingSpinAttack = false;
	private bool doingAttack = false;

	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
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
			if (Scale.X < 3.0f) {
				hurtboxComponent.Monitorable = false;
			}
			else {
				hurtboxComponent.Monitorable = true;
			}

			// Check if the boss is taking damage
			takingDamage = healthComponent.takingDamage;

			if (takingDamage) {
				animPlayer.Play("Hurt");
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
				// SwitchAttack();
				SpinAttack();

				if (doingSpinAttack && Position == frontPosition) {
					camera.ApplyShake(20.0f);
					doingSpinAttack = false;
					hurtboxComponent.Monitorable = false;
				}
				else if (!doingSpinAttack && Position == backPosition) {
					doingSpinAttack = true;
				}
			}					

			// if (shootBallAttack) {
			// 	if (!goToPos) {
			// 		goToPos = true;
			// 		targetPosition = new Vector2(-117.0f, 10.0f);
			// 		var direction = GlobalPosition.DirectionTo(targetPosition);
			// 		if (Position != targetPosition)
			// 			velocity = direction * speed;
			// 		goToPos = false;
			// 	}
				
			// }

			Velocity = velocity;
			MoveAndSlide();
		}	
	}

	private void ShootBallAttack() {
		
	}

	private void SpinAttack() {
		if (doingSpinAttack) {
			if (!playingAnim) {
				playingAnim = true;
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
		else {
			gradualSpeed = 0.0f;
			playingAnim = false;
			// animPlayer.Play("Idle");
			animPlayer.Pause();
			// sprite.Rotation = 0.0f;
			var direction = GlobalPosition.DirectionTo(backPosition);
			velocity = direction * 250.0f;
		}
	}

	private async void SwitchAttack() {
		if (!doingAttack) {
			doingAttack = true;
			doingSpinAttack = true;
			await ToSignal(GetTree().CreateTimer(2.5f), SceneTreeTimer.SignalName.Timeout);
			doingSpinAttack = false;
			await ToSignal(GetTree().CreateTimer(2.5f), SceneTreeTimer.SignalName.Timeout);
			doingAttack = false;
		}
		
	}

	private float GetRandomPosition(int length) {
		Random rnd = new Random();
		int num = rnd.Next(length);
		return num; 
	}
}
