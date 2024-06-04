using Godot;
using System;

public partial class Goemon : CharacterBody2D
{
	// Movement variables
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -350.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D bodySprite;
	[Export] private AnimationPlayer animPlayer;

	// Variables for taking damage
	private Vector2 lastDirection = new Vector2(0.0f, 0.0f);
	private int bounces = 0;
	public bool takingDamage = false;

	private bool isAttacking = false;
	//private bool isAirborne = false;

	public override void _Ready()
	{
		bodySprite.FlipH = false;
		animPlayer.Play("Idle");
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
				velocity.Y = JumpVelocity;
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
			// Player bounces up everytime they hit the floor
			if ((IsOnFloor() && bounces < 2) || (!IsOnFloor() && bounces == 0)) {
				bounces++;
				velocity.Y = -205.0f;
			}

			// Player bounces away from the enemy
			velocity.X = -lastDirection.X * Speed;
		}

		if (Input.IsActionJustPressed("attack")) {
			Attacking();
		}		

		//GD.Print(velocity);

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
					return Mathf.MoveToward(Velocity.X, 0, Speed);
			}
			else if (Input.IsActionPressed("walkLeft")) {
				bodySprite.FlipH = true;

				direction = lastDirection = new Vector2(-1.0f, 0.0f);

				if (!isAttacking)
					movementAnimation();
				else if (isAttacking && IsOnFloor())
					return Mathf.MoveToward(Velocity.X, 0, Speed);
			}
		}

		if (direction != Vector2.Zero)
			return direction.X * Speed;						// Move left or right
		else
			return Mathf.MoveToward(Velocity.X, 0, Speed);	// Don't move
	}

	private void movementAnimation() {
		// Play a movement animation based on the player's input
		if (IsOnFloor()) {
			if (Input.IsActionPressed("crouch"))
				animPlayer.Play("Crawl");
			else
				animPlayer.Play("Walk");
		}
	}

	private async void OnAreaEntered(Area2D area) {
		GD.Print("Player entered area");
		if (!takingDamage && area.IsInGroup("Enemy")) {
			takingDamage = true;
			bounces = 0;
			
			animPlayer.Play("Hurt");
			
			// Wait 0.85s before the player can move
			// if (holdingInput) {
			// 	GD.Print("shorter");
			// 	await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
			// }	
			// else {
			// 	GD.Print("longer");
			// 	await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
			// }

			await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
			takingDamage = false;
		}
	}

	private async void Attacking() {
		if (!isAttacking) {
			isAttacking = true;
			
			if (lastDirection.X >= 0.0f) {
				animPlayer.Play("NormalAttackR");
			}
			else {
				animPlayer.Play("NormalAttackL");
			}

			await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);

			isAttacking = false;
		}
	}
}
