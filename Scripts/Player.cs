using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Movement variables
	private float speed;
	[Export] private float baseSpeed = 150.0f;
	private float maxSpeed = 250.0f;

	[Export] private float jumpVelocity = -500.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Sprite2D bodySprite;
	private AnimationPlayer animPlayer;
	private HurtboxComponent hurtboxComponent;
	public HealthComponent healthComponent;
	private AudioComponent audio;
	private GameManager gm;

	// Variables for taking damage
	private Vector2 lastDirection = new Vector2(0.0f, 0.0f);
	private int bounces = 0;
	public bool takingDamage = false;

	// Variables for attacking
	// public bool isHitting = false;
	private bool isAttacking = false;
	private bool playingHurtSFX = false;
	private bool attackingRight = false;
	private bool attackingLeft = false;

	// Variable for running
	private bool holdingRunButton = false;
	private bool holdingJumpButton = false;

	public int playerNum { get; set; }
	public int coins { get; set; }
	public int lives { get; set; }
	public bool isAlive { get; set; }
	public int chara { get; set; }

	private string[] charaName = {"Goemon", "Ebisumaru", "Sasuke"};

	private bool playingDeathAnim = false;

	private bool gotSpawnPos = false;
	private Vector2 spawnPosition;

	public override void _Ready()
	{
		bodySprite = GetNode<Sprite2D>("Sprite2D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		audio = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");

		speed = baseSpeed;

		if (gm.isBossStage) {
			bodySprite.FlipH = true;
		}
		else {
			bodySprite.FlipH = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (healthComponent.health <= 0.0f) {		
			isAlive = false;

			// Stop any horizontal movement	
			velocity.X = 0.0f;

			// Play death animation
			if (!playingDeathAnim) {
				playingDeathAnim = true;

				// Player flies up a bit
				velocity.Y = -300.0f;

				if (lives >= 0) {
					lives--;
					audio.playSFX("res://Sounds/SFX/deathRing.wav", -12.5f);
					audio.playSFX("res://Sounds/SFX/" + charaName[chara] + "/death.wav", -12.5f);
				}				
			}

			// Fall down through the floor
			if (!IsOnFloor()) {
				velocity.Y += 700.0f * (float)delta;

				if (velocity.Y < 0.0f) {
					animPlayer.Play("DeathUp");
				}
				else {
					animPlayer.Play("DeathDown");
				}
			}

			if (!gotSpawnPos) {
				gotSpawnPos = true;
				spawnPosition = GlobalPosition;
				if (lives >= 0 && !gm.isBossStage)
					Respawn();
			}

			Velocity = velocity;
			MoveAndSlide();
		}
		else {
			isAlive = true;
			velocity.X = horizontalMovement();	

			// Add the gravity.
			if (!IsOnFloor()) {
				velocity.Y += gravity * (float)delta;

				// Player can do short hop by not holding the jump button
				if (Input.IsActionJustReleased("jump" + playerNum.ToString()) && velocity.Y < -250.0f) {
					velocity.Y += 1500 * 6.0f * (float)delta;
				}

				// Play jump or fall animations
				if (!isAttacking && !takingDamage) {
					if (velocity.Y < 0.0f) {
						if (chara == 2) {
							animPlayer.Play("Sasuke/Jump");
						}
						else {
							animPlayer.Play("Jump");
						}
					}
					else {
						if (chara == 0) {
							if (animPlayer.CurrentAnimation != "Fall1" && animPlayer.CurrentAnimation != "Fall2") {
								animPlayer.Play("Fall1");
								animPlayer.Queue("Fall2");
							}
						}
						else if (chara == 1 || chara == 2) {
							animPlayer.Play(charaName[chara] +"/Fall");
						}	
					}
				}
			}
			else {
				// Can only jump once off the ground
				if (Input.IsActionJustPressed("jump" + playerNum.ToString())) {
					holdingJumpButton = true;
					velocity.Y = jumpVelocity;
					
					audio.playSFX("res://Sounds/SFX/" + charaName[chara] + "/jump.wav", -20.0f);
				}
				else if (Input.IsActionJustReleased("jump" + playerNum.ToString())) {
					holdingJumpButton = false;
				}
				
				if (velocity.X != 0.0f && holdingRunButton) {
					if (speed < maxSpeed)
						speed += 1.0f;				// Gradually increase speed to maxSpeed
				}
				else if (!holdingRunButton) {
					if (speed > baseSpeed)
						speed -= 5.0f;				// Speed drops to baseSpeed quicker
					else if (speed <= baseSpeed)
						speed = baseSpeed;			// Make sure speed does not go below baseSpeed
				}

				// Play one of these animations if the player is not moving
				if (velocity.X == 0.0f && !isAttacking && !takingDamage) {
					if (Input.IsActionPressed("crouch" + playerNum.ToString())) {
						animPlayer.Play("Crouch");
					}
					else if (Input.IsActionPressed("lookUp" + playerNum.ToString())) {
						animPlayer.Play("LookUp");
					}
					else {
						animPlayer.Play("Idle");
					}
				}	
			}

			if (takingDamage) {
				animPlayer.Play("Hurt");			
				Hurt();
				
				// Player bounces up everytime they hit the floor
				if ((IsOnFloor() && bounces < 2) || (!IsOnFloor() && bounces == 0)) {
					bounces++;
					velocity.Y = -205.0f;
				}

				// Player bounces away from the enemy
				speed = baseSpeed;
				velocity.X = -lastDirection.X * speed;
			}
			else {
				bounces = 0;
			}

			// For running
			if (Input.IsActionPressed("attack" + playerNum.ToString())) {
				Attacking();
				holdingRunButton = true;
			}
			else if (Input.IsActionJustReleased("attack" + playerNum.ToString())) {
				holdingRunButton = false;
			}

			takingDamage = hurtboxComponent.takingDamage;

			Velocity = velocity;
			MoveAndSlide();		
		}
	}

	/*
	 * Player can move when not taking damage or attacking
	 * Stop movement when attacking on the ground
	 * You can move in the direction you are attacking towards in the air
	 * Player can't turn during the attack animation 
	*/
	private float horizontalMovement() {
		Vector2 direction = Vector2.Zero;

		// Player can only move when not attacking or taking damage
		if (!takingDamage) {
			if (Input.IsActionPressed("walkRight" + playerNum.ToString()) && !attackingLeft) {
				// Turn the sprite towards the right direction
				bodySprite.FlipH = false;
				
				// Set the direction of the player
				direction = lastDirection = new Vector2(1.0f, 0.0f);

				if (!isAttacking) {
					// Play movement animation if not attacking
					movementAnimation();
				}
				else if (isAttacking && IsOnFloor()) {
					// Stop player if they attack on the ground
					return Mathf.MoveToward(Velocity.X, 0, speed);
				}					
			}
			else if (Input.IsActionPressed("walkLeft" + playerNum.ToString()) && !attackingRight) {
				bodySprite.FlipH = true;

				direction = lastDirection = new Vector2(-1.0f, 0.0f);

				if (!isAttacking) {
					movementAnimation();
				}
				else if (isAttacking && IsOnFloor()) {
					return Mathf.MoveToward(Velocity.X, 0, speed);
				}			
			}
		}

		// Return the velocity of the player in the x-direction
		if (direction != Vector2.Zero)
			return direction.X * speed;
		else
			return Mathf.MoveToward(Velocity.X, 0, speed);
	}

	private void movementAnimation() {
		// Play a movement animation based on the player's input
		if (IsOnFloor()) {
			if (Input.IsActionPressed("crouch" + playerNum.ToString()))
				animPlayer.Play("Crawl");
			else if (holdingRunButton)
				animPlayer.Play("Run");
			else if (!holdingRunButton)
				animPlayer.Play("Walk");
		}
	}

	private async void Attacking() {
		if (!isAttacking && !takingDamage && !holdingRunButton) {
			isAttacking = true;

			audio.playSFX("res://Sounds/SFX/" + charaName[chara] + "/attack.wav", -20.0f);

			// Used for different animation times
			float time = 0.25f;
			if (lastDirection.X >= 0.0f) {
				attackingRight = true;

				// Play appropriate animation for the character
				if (Input.IsActionPressed("crouch" + playerNum.ToString()) && IsOnFloor()) {
					animPlayer.Play(charaName[chara] +"/CrouchAttackR");
					if (chara == 2) {
						time = 0.35f;
					}
				}
				else if (Input.IsActionPressed("lookUp" + playerNum.ToString())) {
					if (chara == 2) {
						if (IsOnFloor()) {
							animPlayer.Play("Sasuke/UpAttackR");
							time = 0.35f;
						}
						else
							animPlayer.Play("Sasuke/NormalAttackR");
					}
					else if (chara != 2) {
						animPlayer.Play(charaName[chara] +"/UpAttackR");
					}
				}
				else {
					animPlayer.Play(charaName[chara] +"/NormalAttackR");
				}
			}
			else {
				attackingLeft = true;

				// Play appropriate animation for the character
				if (Input.IsActionPressed("crouch" + playerNum.ToString()) && IsOnFloor()) {
					animPlayer.Play(charaName[chara] +"/CrouchAttackL");
					if (chara == 2) {
						time = 0.35f;
					}
				}
				else if (Input.IsActionPressed("lookUp" + playerNum.ToString())) {
					if (chara == 2) {
						if (IsOnFloor()) {
							animPlayer.Play("Sasuke/UpAttackL");
							time = 0.35f;
						}
						else
							animPlayer.Play("Sasuke/NormalAttackL");
					}
					else {
						animPlayer.Play(charaName[chara] +"/UpAttackL");
					}
				}
				else {
					animPlayer.Play(charaName[chara] +"/NormalAttackL");
				}
			}

			// Wait for animation to finish
			await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);

			attackingRight = false;
			attackingLeft = false;
			isAttacking = false;
		}
	}

	// Only plays the hurt sound once
	private async void Hurt() {
		if (!playingHurtSFX) {
			playingHurtSFX = true;
			audio.playSFX("res://Sounds/SFX/" + charaName[chara] + "/hurt.wav", -12.5f);
			await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
			playingHurtSFX = false;
		}
	}

	private async void Respawn() {
		await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);
		Velocity = new Vector2(0.0f, 0.0f);
		GlobalPosition = spawnPosition;
		healthComponent.health = 12.0f;
		animPlayer.Play("Idle");
		gotSpawnPos = false;
		playingDeathAnim = false;
	}

	public void Die() {
		healthComponent.health = 0.0f;
	}
}