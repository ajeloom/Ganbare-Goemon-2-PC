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
				if (Input.IsActionJustReleased("jump") && velocity.Y < -250.0f) {
				// if (!Input.IsJoyButtonPressed(playerNum - 1, JoyButton.A) && velocity.Y < -250.0f) {
					velocity.Y += 1500 * 6.0f * (float)delta;
				}

				// Play jump or fall animations
				if (!isAttacking && !takingDamage) {
					if (velocity.Y < 0.0f) {
						animPlayer.Play("Jump");
					}
					else {
						if (chara == 0) {
							if (animPlayer.CurrentAnimation != "Fall1" && animPlayer.CurrentAnimation != "Fall2") {
								animPlayer.Play("Fall1");
								animPlayer.Queue("Fall2");
							}
						}
						else if (chara == 1) {
							animPlayer.Play("Ebisumaru/Fall");
						}
							
					}
				}
			}
			else {
				// Can only jump once off the ground
				if (Input.IsActionJustPressed("jump")) {
				// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.A) && !holdingJumpButton) {
					holdingJumpButton = true;
					velocity.Y = jumpVelocity;
					
					audio.playSFX("res://Sounds/SFX/" + charaName[chara] + "/jump.wav", -20.0f);
				}
				else if (Input.IsActionJustReleased("jump")) {
				// else if (!Input.IsJoyButtonPressed(playerNum - 1, JoyButton.A)) {
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
					if (Input.IsActionPressed("crouch")) {
					// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadDown)) {
						animPlayer.Play("Crouch");
					}
					else if (Input.IsActionPressed("lookUp")) {
					// else if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadUp)) {
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
			if (Input.IsActionPressed("attack")) {
			// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.X)) {
				Attacking();
				holdingRunButton = true;
			}
			else if (Input.IsActionJustReleased("attack")) {
			// else if (!Input.IsJoyButtonPressed(playerNum - 1, JoyButton.X)) {
				holdingRunButton = false;
			}

			takingDamage = hurtboxComponent.takingDamage;

			Velocity = velocity;
			MoveAndSlide();		
		}
	}

	private float horizontalMovement() {
		Vector2 direction = Vector2.Zero;

		// - Move when not taking damage or attacking
		// X - Stop moving when attacking on the ground
		// - You can't move while attacking in the air unless you are already moving before attacking
		// - Can't turn during the attack animation 

		// Player can only move when not attacking or taking damage
		if (!takingDamage) {
			// Can only turn when not attacking
			if (Input.IsActionPressed("walkRight")) {
			// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadRight)) {
				bodySprite.FlipH = false;
				
				direction = lastDirection = new Vector2(1.0f, 0.0f);

				if (!isAttacking)
					movementAnimation();
				else if (isAttacking && IsOnFloor())
					return Mathf.MoveToward(Velocity.X, 0, speed);
			}
			else if (Input.IsActionPressed("walkLeft")) {
			// else if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadLeft)) {
				bodySprite.FlipH = true;

				direction = lastDirection = new Vector2(-1.0f, 0.0f);

				if (!isAttacking)
					movementAnimation();
				else if (isAttacking && IsOnFloor())
					return Mathf.MoveToward(Velocity.X, 0, speed);
			}
		}

		if (direction != Vector2.Zero)
			return direction.X * speed;
		else
			return Mathf.MoveToward(Velocity.X, 0, speed);
	}

	private void movementAnimation() {
		// Play a movement animation based on the player's input
		if (IsOnFloor()) {
			if (Input.IsActionPressed("crouch"))
			// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadDown))
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
			if (lastDirection.X >= 0.0f) {
				// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadDown) && IsOnFloor())
				if (Input.IsActionPressed("crouch") && IsOnFloor()) {
					if (chara == 0)
						animPlayer.Play("CrouchAttackR");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/CrouchAttackR");
				}
				// else if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadUp))
				else if (Input.IsActionPressed("lookUp")) {
					if (chara == 0)
						animPlayer.Play("UpAttackR");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/UpAttackR");
				}
				else {
					if (chara == 0)
						animPlayer.Play("NormalAttackR");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/NormalAttackR");
				}
			}
			else {
				// if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadDown) && IsOnFloor())
				if (Input.IsActionPressed("crouch") && IsOnFloor()) {
					if (chara == 0)
						animPlayer.Play("CrouchAttackL");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/CrouchAttackL");
				}
				// else if (Input.IsJoyButtonPressed(playerNum - 1, JoyButton.DpadUp))
				else if (Input.IsActionPressed("lookUp")) {
					if (chara == 0)
						animPlayer.Play("UpAttackL");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/UpAttackL");
				}
				else {
					if (chara == 0)
						animPlayer.Play("NormalAttackL");
					else if (chara == 1)
						animPlayer.Play("Ebisumaru/NormalAttackL");
				}
			}
				
			await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);

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