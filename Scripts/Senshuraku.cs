using Godot;
using System;

public partial class Senshuraku : CharacterBody2D
{
	[Export] public float speed = 100.0f;
	
	private Vector2 backPosition = new Vector2(0.0f, 7.0f);
	private Vector2 frontPosition = new Vector2(0.0f, 80.0f);
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
	private bool startUpBall = true;
	private bool startUpPassed = false;
	private bool gotRandomAttack = false;

	private bool playedHurtSound = false;

	private AnimationPlayer animPlayer;
	public HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private HitboxComponent hitboxComponent;
	private AudioComponent audio;
	private Sprite2D sprite;
	private ScreenShake camera;
	private CollisionShape2D hurtbox;

	private Impact impact;

	private bool takingDamage = false;
	private bool goBack = false;

	private bool isLevelEnding = false;

	private Vector2 velocity;
	private float gradualSpeed = 0.0f;

	public override void _Ready() {
		Position = new Vector2(0.0f, 7.0f);
		Scale = new Vector2(0.7f, 0.7f);

		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		audio = GetNode<AudioComponent>("AudioComponent");
		sprite = GetNode<Sprite2D>("Sprite2D");
		camera = GetNode<ScreenShake>("/root/Impact Battle/Camera2D");
		impact = GetNode<Impact>("/root/Impact Battle/Impact");
		hurtbox = hurtboxComponent.GetNode<CollisionShape2D>("Hurtbox");
		hurtbox.Disabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		velocity = Velocity;

		if (healthComponent.health <= 0.0f) {
			if (!playedHurtSound) {
				playedHurtSound = true;
				audio.playSFX("res://Sounds/SFX/punch.wav", -15.0f);
			}

			EndLevel();
		}
		else {
			if (!startUpPassed) {
				StartUp();
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
				if (Scale.X < 6.5f || goBack) {
					hurtbox.Disabled = true;
				}
				else {
					hurtbox.Disabled = false;
				}

				// Check if the boss is taking damage
				takingDamage = hurtboxComponent.tookDamage;

				if (takingDamage) {
					animPlayer.Play("Hurt");
					if (!playedHurtSound) {
						playedHurtSound = true;
						audio.playSFX("res://Sounds/SFX/punch.wav", -15.0f);
					}
					doingBallAttack = false;
					doingSpinAttack = false;
					gradualSpeed = 0.0f;
					Vector2 direction = GlobalPosition.DirectionTo(backPosition);
					velocity = direction * 200.0f;
					if (Position.Y <= backPosition.Y) {
						camera.ApplyShake(2.0f);
					}
				}
				else {
					playedHurtSound = false;
					if (impact.isAlive) {
						if (!doingBallAttack && !doingSpinAttack) {
							playingSpinAnim = false;
							playingBallAnim = false;
							GetRandomAttack();
						}

						if (doingSpinAttack) {
							SpinAttack();
						}
						else {
							gradualSpeed = 0.0f;
						}
						
						if (doingBallAttack) {
							ShootBallAttack();
						}
					}
					else {
						// Freeze when player is not alive
						velocity.X = 0.0f;
						animPlayer.Play("Idle");
						camera.ApplyShake(15.0f);
					}
				}		

				Velocity = velocity;
				MoveAndSlide();
			}
		}	
	}

	private async void ShootBallAttack() {
		if (startUpBall) {
			if (!playingBallAnim) {
				playingBallAnim = true;
				animPlayer.Play("SpinStartUp");

				// Pick a side to move towards
				targetPosition = new Vector2(ChooseSide(), 7.0f);
				speed = 250.0f;
			}			

			if (targetPosition.X == -120.0f) {
				if (Position.X <= targetPosition.X) {
					startUpBall = false;
				}
			}
			else if (targetPosition.X == 120.0f) {
				if (Position.X >= targetPosition.X) {
					startUpBall = false;
				}
			}
		}
		else {
			// Shoot 4 ball projectiles
			if (!spawnedBalls) {
				spawnedBalls = true;

				// Go to the other side
				targetPosition = new Vector2(-targetPosition.X, 7.0f);
				speed = 100.0f;

				for (int i = 0; i < 4; i++) {
					animPlayer.Play("BallAttack");
					LoadScene("res://Scenes/Ball.tscn", Position.X, 7.0f);
					await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
					animPlayer.Play("Idle");
					await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
				}
			}

			if (targetPosition.X == -120.0f) {
				if (Position.X <= targetPosition.X) {
					doingBallAttack = false;
					spawnedBalls = false;
				}
			}
			else if (targetPosition.X == 120.0f) {
				if (Position.X >= targetPosition.X) {
					doingBallAttack = false;
					spawnedBalls = false;
				}
			}
		}
		
		Vector2 direction = GlobalPosition.DirectionTo(targetPosition);
		velocity = direction * speed;
	}

	private void SpinAttack() {
		// Go to the middle of the screen before starting attack
		if (Position.X >= 5.0f || Position.X <= -5.0) {
			Vector2 direction = GlobalPosition.DirectionTo(new Vector2(0.0f, 7.0f));			
			velocity = direction * 100.0f;
		}
		else {
			if (!playingSpinAnim) {
				playingSpinAnim = true;
				animPlayer.Play("SpinAttack");
			}

			Vector2 direction = GlobalPosition.DirectionTo(frontPosition);			
			velocity = direction * gradualSpeed;

			if (gradualSpeed < 50.0f) {
				gradualSpeed += 2.0f;
			}
			else if (gradualSpeed >= 50.0f) {
				gradualSpeed = 50.0f;
			}

			// Boss lands the spin attack hit
			if (Position.Y == frontPosition.Y) {
				if (!goBack) {
					goBack = true;
					animPlayer.Pause();
					camera.ApplyShake(20.0f);
					HealthComponent hp = GetNode<HealthComponent>("/root/Impact Battle/Impact/HealthComponent");
					hp.Damage(20, hitboxComponent);

					LoadScene("res://Scenes/Crack.tscn", Position.X, Position.Y - 80.0f);
				}
			}

			// Move back to the middle after hitting player
			if (goBack) {
				direction = GlobalPosition.DirectionTo(backPosition);			
				velocity = direction * 125.0f;
				if (Position.Y <= backPosition.Y) {
					doingSpinAttack = false;
					goBack = false;
				}
			}
		}
	}

	private void LoadScene(string name, float xPos, float yPos) {
		PackedScene scene = GD.Load<PackedScene>(name);
		Node2D instance = scene.Instantiate<Node2D>();
		instance.GlobalPosition = new Vector2(xPos, yPos);
		AddSibling(instance, true);
	}

	private void GetRandomAttack() {
		if (!gotRandomAttack) {
			gotRandomAttack = true;
			Random rnd = new Random();
			int num = rnd.Next(2);
			
			if (num == 0) {
				startUpBall = true;
				doingBallAttack = true;
				doingSpinAttack = false;
				
			}
			else if (num == 1) {
				doingBallAttack = false;
				doingSpinAttack = true;
			}
			gotRandomAttack = false;
		}	
	}

	private float ChooseSide() {
		Random rnd = new Random();
		int negative = rnd.Next(2);
		if (negative == 0) {
			return -120.0f;
		}
		return 120.0f; 
	}

	private async void StartUp() {
		// Wait one second
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		startUpPassed = true;
	}

	private async void EndLevel() {
		if (!isLevelEnding) {
			isLevelEnding = true;
			GameManager.instance.canPause = false;

			animPlayer.Play("Hurt");
			GameManager.instance.audio.Stop();

			for (int i = 0; i <= 5; i++) {
				PackedScene scene = GD.Load<PackedScene>("res://Scenes/Explosion.tscn");
				Explosion instance = scene.Instantiate<Explosion>();
				if (i < 5) {
					instance.explosion = 0;
					camera.ApplyShake(5.0f);
				}
				else {
					instance.explosion = 2;
					camera.ApplyShake(30.0f);
					Visible = false;
				}
					
				AddSibling(instance, true);
				await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);
			}

			await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

			// Load stage clear music
			GameManager.instance.audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/StageClear.mp3");
			GameManager.instance.audio.VolumeDb = 0.0f;
			GameManager.instance.audio.Play();
			
			await ToSignal(GetTree().CreateTimer(4.5f), SceneTreeTimer.SignalName.Timeout);
			GameManager.instance.GoToMenu();
		}
		
	}
}


