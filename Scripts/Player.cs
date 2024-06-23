using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Movement variables
	public float speed;
	[Export] private float baseSpeed = 150.0f;
	private float maxSpeed = 250.0f;

	[Export] public float jumpVelocity = -500.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D bodySprite;
	[Export] private AnimationPlayer animPlayer;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] public HealthComponent healthComponent;
	[Export] private AudioComponent audio;

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

	public int coins = 100;
	public int lives = 2;

	[Export] public int playerNum = 1;

	public override void _Ready()
	{
		speed = baseSpeed;
		bodySprite.FlipH = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		velocity.X = horizontalMovement();	

		// Add the gravity.
		if (!IsOnFloor()) {
			velocity.Y += gravity * (float)delta;

			// Play jump or fall animations
			if (!isAttacking && !takingDamage) {
				if (velocity.Y < 0.0f) {
					animPlayer.Play("Jump");
				}
				else {
					if (animPlayer.CurrentAnimation != "Fall1" && animPlayer.CurrentAnimation != "Fall2") {
						animPlayer.Play("Fall1");
						animPlayer.Queue("Fall2");
					}	
				}
			}
		}
		else {
			// Can only jump once off the ground
			if (Input.IsActionJustPressed("jump")) {
				//holdingJump = true;
				velocity.Y = jumpVelocity;
				
				audio.playSFX("res://Sounds/SFX/Goemon/jump.wav", -20.0f);
			}
			// else if (Input.IsActionJustReleased("jump")) {
			// 	holdingJump = false;
			// }
			
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
					animPlayer.Play("Crouch");
				}
				else if (Input.IsActionPressed("lookUp")) {
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
		if (Input.IsActionJustPressed("attack")) {
			holdingRunButton = true;
			Attacking();
		}
		else if (Input.IsActionJustReleased("attack")) {
			holdingRunButton = false;
		}

		// if (holdingJump) {
		// 	//if (jumpVelocity > 0.0f)
		// 	gravity -= 100.0f;
		// }
		// else {
				
		// }

		takingDamage = hurtboxComponent.takingDamage;

		Velocity = velocity;
		MoveAndSlide();
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
				bodySprite.FlipH = false;
				
				direction = lastDirection = new Vector2(1.0f, 0.0f);

				if (!isAttacking)
					movementAnimation();
				else if (isAttacking && IsOnFloor())
					return Mathf.MoveToward(Velocity.X, 0, speed);
			}
			else if (Input.IsActionPressed("walkLeft")) {
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
				animPlayer.Play("Crawl");
			else if (holdingRunButton)
				animPlayer.Play("Run");
			else if (!holdingRunButton)
				animPlayer.Play("Walk");
		}
	}

	private async void Attacking() {
		if (!isAttacking && !takingDamage) {
			isAttacking = true;

			audio.playSFX("res://Sounds/SFX/Goemon/attack.wav", -20.0f);
			if (lastDirection.X >= 0.0f)
				animPlayer.Play("NormalAttackR");
			else 
				animPlayer.Play("NormalAttackL");

			await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);

			isAttacking = false;
		}
	}

	// Only plays the hurt sound once
	private async void Hurt() {
		if (!playingHurtSFX) {
			playingHurtSFX = true;
			audio.playSFX("res://Sounds/SFX/Goemon/hurt.wav", -12.5f);
			await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
			playingHurtSFX = false;
		}
	}
}