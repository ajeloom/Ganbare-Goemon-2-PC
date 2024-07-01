using Godot;
using System;

public partial class Kabuki : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	public const float JumpVelocity = -420.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// For start up animation
	private bool oneSec = true;
	private bool startUpAnimation = true;
	private Vector2 targetPosition;
	private bool emerged = false;

	// For shaking the camera
	public bool landed = false;
	private bool playLandedAnimation = false;
	public bool shakeCamera = false;

	// For phase 1/2 attacks
	private bool isFlying = false;
	private bool startFlyingAttack = false;
	private bool spawnFlowers = false;
	private bool move = false;
	private bool moveLeft = true;
	private bool moveRight = false;
	private bool moveUp = true;
	private bool moveDown = false;

	// Components/Nodes
	private GameManager gm;
	private AnimationPlayer animPlayer;
	//[Export] private AnimationPlayer effectsPlayer;
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private AudioComponent audioComponent;
	private RayCast2D rayCast;
	private Sprite2D body;
	private ScreenShake camera;

	private int phase = 1;

	private bool createNewHP = false;

	private bool takingDamage = false;

	private bool endLevel = false;

	public override void _Ready()
	{
		Position = new Vector2(320.0f, 100.0f);

		targetPosition = new Vector2(320.0f, 140.0f);

		var scene = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
		var instance = scene.Instantiate();
		AddChild(instance);

		gm = GetNode<GameManager>("/root/GameManager");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");	
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hurtboxComponent.Scale = Vector2.Zero;
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		rayCast = GetNode<RayCast2D>("RayCast2D");
		body = GetNode<Sprite2D>("Body");
		camera = GetNode<ScreenShake>("/root/Boss Fight/Camera2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Wait a second before moving
		if (oneSec) {
			startUp();
		}
		else {
			Vector2 velocity = Velocity;

			if (healthComponent.health > 0.0f) {
				if (phase == 1) {
					// Fall when in the air and not flying
					if (!IsOnFloor() && !isFlying) {
						float currentGravity = (startUpAnimation) ? 35.0f : gravity;
						velocity.Y += currentGravity * (float)delta;
						landed = false;
					}

					// Do startup animation
					if (Position.Y >= 140.0f && startUpAnimation) {
						hurtboxComponent.Scale = new Vector2(1.0f, 1.0f);
						if (!emerged) {
							emerged = true;
							animPlayer.Play("emerge");
						}
					}

					if (IsOnFloor()) {
						// Start up animation ends
						startUpAnimation = false;

						// Shake the camera when landing
						if (!landed) {
							landed = true;
							camera.ApplyShake(2.0f);
						}

						// Do flower attack
						doFlowerAttack();
					}

					// Fly in the air to target position
					if (animPlayer.CurrentAnimation == "hide") {
						targetPosition = new Vector2(Position.X, 140.0f);
						Vector2 direction = GlobalPosition.DirectionTo(targetPosition);
						
						if (Position >= targetPosition) {
							velocity = direction * 100.0f;
							isFlying = true;
						}
					}

					// Reset to target position for flying attack
					if (isFlying && Position <= targetPosition) {
						velocity = Vector2.Zero;
						animPlayer.Play("RESET");
						startFlyingAttack = true;
					}

					// Do flying attack
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
					// Fall when in the air
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
							camera.ApplyShake(2.0f);
						}
						velocity.Y = JumpVelocity;
						doJumpingAttack();

						// Move left and right
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
			}
			else {
				if (phase == 1) {
					if (!createNewHP) {
						createNewHP = true;
						healthComponent.health = 32.0f;
						var scene = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
						var instance = scene.Instantiate();
						AddChild(instance);
					}
					
					// Spawn a basket with explosion
					PackedScene var = GD.Load<PackedScene>("res://Scenes/Basket.tscn");
					Node2D node = var.Instantiate<Node2D>();
					node.Position = Position;
					AddSibling(node);

					isFlying = false;
					startUpAnimation = false;
					velocity.X = 0.0f;

					phase = 2;
				}
				else if (phase == 2) {
					if (!IsOnFloor()) {
						velocity.Y += gravity * (float)delta;
						landed = false;
					}

					velocity.X = 0.0f;
					EndLevel();
				}
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
		// startUpAnimation = false;

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

				// Shoot a group of 8 flowers, 3 times
				for (int j = 0; j < 3; j++) {
					for (int i = 0; i < 8; i++) {
						// Position them in evenly above the boss
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
		else if (animPlayer.CurrentAnimation == "close") {
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

	private async void EndLevel() {
		if (!endLevel) {
			endLevel = true;

			animPlayer.Play("death");

			for (int i = 0; i < 6; i++) {
				audioComponent.playSFX("res://Sounds/SFX/explosion.wav", -15.0f);
				await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);
			}

			// Load stage clear music
			gm.audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/StageClear.mp3");
			gm.audio.VolumeDb = 0.0f;
			gm.audio.Play();
			
			await ToSignal(GetTree().CreateTimer(4.5f), SceneTreeTimer.SignalName.Timeout);
			gm.endLevel = true;
		}
		
	}
}
