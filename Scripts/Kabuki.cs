using Godot;
using System;

public partial class Kabuki : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	public float descendSpeed = 15.0f;
	public const float JumpVelocity = -420.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool oneSec = true;
	private bool startUpAnimation = true;
	private bool startUpFall = true;
	private Vector2 targetPosition;

	private bool emerged = false;
	private bool descendSlowly = false;

	public bool landed = false;
	private bool playLandedAnimation = false;

	public bool shakeCamera = false;

	private bool isFlying = false;
	private bool startFlyingAttack = false;

	private bool spawnFlowers = false;

	[Export] private AnimationPlayer animPlayer;
	//[Export] private AnimationPlayer effectsPlayer;
	[Export] private RayCast2D rayCast;
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private AudioComponent audioComponent;

	[Export] private Sprite2D body;

	[Export] ScreenShake camera;

	private int phase = 1;

	private bool move = false;
	private bool moveLeft = true;
	private bool moveRight = false;
	private bool moveUp = true;
	private bool moveDown = false;

	private bool createNewHP = false;

	private bool takingDamage = false;

	// Different Attacks
	// Shoot flowers from basket then go in basket and fly a bit
	// Once basket breaks
	// Jump

	public override void _Ready()
	{
		Position = new Vector2(320.0f, 100.0f);

		targetPosition = new Vector2(320.0f, 140.0f);

		var scene = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
		var instance = scene.Instantiate();
		AddChild(instance);

		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hurtboxComponent.Scale = Vector2.Zero;

		audioComponent = GetNode<AudioComponent>("AudioComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Wait a second before moving
		if (oneSec) {
			startUp();
		}
		else {
			Vector2 velocity = Velocity;

			if (phase == 1) {
				if (!IsOnFloor() && !isFlying) {
					float currentGravity = (startUpAnimation) ? 35.0f : gravity;
					velocity.Y += currentGravity * (float)delta;
					landed = false;
				}

				if (Position.Y >= 140.0f && startUpAnimation) {
					hurtboxComponent.Scale = new Vector2(1.0f, 1.0f);
					if (!emerged) {
						emerged = true;
						animPlayer.Play("emerge");
					}
				}

				// Start up animation ends when boss lands on the ground
				if (IsOnFloor()) {
					// Shake the camera when the boss lands on the floor
					if (!landed) {
						landed = true;
						camera.ApplyShake();
					}

					doFlowerAttack();
				}

				if (animPlayer.CurrentAnimation == "hide") {
					targetPosition = new Vector2(Position.X, 140.0f);
					Vector2 direction = GlobalPosition.DirectionTo(targetPosition);
					
					if (Position >= targetPosition) {
						velocity = direction * 100.0f;
						isFlying = true;
						landed = false;
					}
					
				}

				// Reset to target position for flying attack
				if (isFlying && Position <= targetPosition) {
					velocity = Vector2.Zero;
					animPlayer.Play("RESET");
					startFlyingAttack = true;
				}

				if (startFlyingAttack) {
					playLandedAnimation = false;
					doFlyingAttack();

					if (move) {
						if (moveLeft) {
							velocity.X = -1.0f * Speed;
						}	
						else if (moveRight) {
							velocity.X = 1.0f * Speed;
						}
					}
					else {
						velocity.X = 0.0f;
					}

					// if (moveUp && isFlying) {
					// 	velocity.Y = -1.0f * Speed;
					// }

					// if (moveDown && isFlying) {
					// 	velocity.Y = 1.0f * Speed;
					// }
				}
			}
			else if (phase == 2) {
				if (!createNewHP) {
					createNewHP = true;
					healthComponent.health = 32.0f;
					var scene = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
					var instance = scene.Instantiate();
					AddChild(instance);
				}

				if (!IsOnFloor()) {
					if (!startUpAnimation) {
						startUpAnimation = true;
						animPlayer.Play("land");
					}
					velocity.Y += gravity * (float)delta;
					landed = false;
				}

				// Switch to jumping attack
				if (IsOnFloor()) {
					if (!landed) {
						landed = true;
						camera.ApplyShake();
					}
					velocity.Y = JumpVelocity;
					doJumpingAttack();

					if (move) {
						if (moveLeft) {
							velocity.X = -1.0f * Speed;
						}	
						else if (moveRight) {
							velocity.X = 1.0f * Speed;
						}
					}
					else {
						velocity.X = 0.0f;
					}

					animPlayer.Play("jump");
					audioComponent.playSFX("res://Sounds/SFX/Goemon/jump.wav", -25.0f);
				}

				if (takingDamage) {
					animPlayer.Play("hurt");
				}

				
			}

			if (healthComponent.health <= 0.0f && phase == 1) {
				phase = 2;
				isFlying = false;
				startUpAnimation = false;
				velocity.X = 0.0f;
			}
			else if (healthComponent.health <= 0.0f && phase == 2) {
				QueueFree();
			}

			takingDamage = hurtboxComponent.takingDamage;

			Velocity = velocity;
			MoveAndSlide();
		}	
	}

	private async void startUp() {
		// Wait one second
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		oneSec = false;
	}
	

	private async void doFlowerAttack() {
		// Gravity is normal
		startUpAnimation = false;

		if (!playLandedAnimation) {
			playLandedAnimation = true;
			animPlayer.Play("land");
			animPlayer.Queue("open");
			animPlayer.Queue("close");
			animPlayer.Queue("hide");	
		}

		// Spawn flowers
		if (animPlayer.CurrentAnimation == "open") {	
			if (!spawnFlowers) {
				spawnFlowers = true;
				await ToSignal(GetTree().CreateTimer(0.9f), SceneTreeTimer.SignalName.Timeout);
				int temp = 1;
				for (int j = 0; j < 3; j++) {
					for (int i = 0; i < 8; i++) {
						PackedScene var = GD.Load<PackedScene>("res://Scenes/Flower.tscn");
						Node2D node = var.Instantiate<Node2D>();
						if (i < 4) {
							node.Position = new Vector2(Position.X - (6 * temp), Position.Y - 57.5f);
							temp += 1;
						}
						else {
							node.Position = new Vector2(Position.X + (6 * temp), Position.Y - 57.5f);
							temp += 1;
						}

						if (temp == 5) {
							temp = 1;
						}
						AddSibling(node);
					}
					await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				}
				
			}			
		}

		if (animPlayer.CurrentAnimation == "close") {
			spawnFlowers = false;
		}
	}

	private void doFlyingAttack() {
		// Go left first
		// Then go right if you hit a wall
		move = true;

		if (Position.X <= 59.0f) {
			moveLeft = false;
			moveRight = true;
			body.FlipH = true;
		}
		else if (Position.X >= 581.0f) {
			moveLeft = true;
			moveRight = false;
			body.FlipH = false;
		}

		/*
		if (Position.Y <= 180.0f) {
			moveUp = false;
			moveDown = true;
		}
		else if (Position.Y >= 100.0f) {
			moveUp = true;
			moveDown = false;
		}
		*/

		// Drop on the player when above
		if (rayCast.IsColliding()) {
			move = false;
			isFlying = false;
			startFlyingAttack = false;
			animPlayer.Play("emerge");
		}
	}

	private void doJumpingAttack() {
		move = true;

		if (Position.X <= 59.0f) {
			moveLeft = false;
			moveRight = true;
			body.FlipH = true;
		}
		else if (Position.X >= 581.0f) {
			moveLeft = true;
			moveRight = false;
			body.FlipH = false;
		}
	}
}
