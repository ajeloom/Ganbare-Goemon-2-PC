using Godot;
using System;

public partial class Goemon : CharacterBody2D
{
	// Movement variables
	[Export] public float speed = 170.0f;
	[Export] public float jumpVelocity = -500.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D bodySprite;
	[Export] private AnimationPlayer animPlayer;
	[Export] private HurtboxComponent hurtboxComponent;
	[Export] private AudioStreamPlayer2D audio;

	// Variables for taking damage
	private Vector2 lastDirection = new Vector2(0.0f, 0.0f);
	private int bounces = 0;
	public bool takingDamage = false;

	// Variable for attacking
	private bool isAttacking = false;

	// Variable for running
	private bool isRunning = false;

	public override void _Ready()
	{
		bodySprite.FlipH = false;
		animPlayer.Play("Idle");
		audio.VolumeDb = 0.0f;
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
				velocity.Y = jumpVelocity;
				playSFX("jump");
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
			// Player bounces up everytime they hit the floor
			if ((IsOnFloor() && bounces < 2) || (!IsOnFloor() && bounces == 0)) {
				bounces++;
				velocity.Y = -205.0f;
			}

			// Player bounces away from the enemy
			velocity.X = -lastDirection.X * speed;
		}
		else {
			bounces = 0;
		}

		// For running
		if (Input.IsActionJustPressed("attack")) {
			if (IsOnFloor()) {
				isRunning = true;
			}
			Attacking();
		}
		else if (Input.IsActionJustReleased("attack")) {
			isRunning = false;
		}

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

			if (isRunning) {
				if (speed < 250.0f)
					speed += 2.0f;
			}
			else {
				if (speed != 170.0f)
					speed -= 2.0f;
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
			else if (isRunning) {
				animPlayer.Play("Run");
			}
			else if (!isRunning)
				animPlayer.Play("Walk");
		}
	}

	private async void Attacking() {
		if (!isAttacking) {
			isAttacking = true;
			
			if (lastDirection.X >= 0.0f)
				animPlayer.Play("NormalAttackR");
			else 
				animPlayer.Play("NormalAttackL");

			await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);

			isAttacking = false;
		}
	}

	private void playSFX(string action) {
		if (action == "jump") {
			audio.Stream = (AudioStream)ResourceLoader.Load("res://SFX/jump.wav");
			audio.VolumeDb = -12.5f;
		}

		audio.Play();
	}
}
