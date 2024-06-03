using Godot;
using System;

public partial class Goemon : CharacterBody2D
{
	// Movement variables
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -350.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D sprite;
	[Export] private AnimationPlayer animPlayer;

	// Variables for taking damage
	private Vector2 lastDirection;
	private int bounces = 0;
	public bool takingDamage = false;
	private bool holdingInput = false;

	public override void _Ready()
	{
		sprite.FlipH = false;	
		animPlayer.Play("Idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		handleInput();
		if (!takingDamage) {
			handleMovement(delta);
		}
		else {
			tookDamage(delta);
		}
	}

	private void handleMovement(double delta) {
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor()) {
			velocity.Y += gravity * (float)delta;

			// The player is in the air
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

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
			animPlayer.Play("Jump");
		}
			
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Vector2.Zero;
		if (Input.IsActionPressed("walkRight")) {
			sprite.FlipH = false;
			direction = new Vector2(1.0f, 0.0f);
			lastDirection = direction;
			
			if (IsOnFloor())
				selectAnimationForWalking();
		}
		else if (Input.IsActionPressed("walkLeft")) {
			sprite.FlipH = true;
			direction = new Vector2(-1.0f, 0.0f);
			lastDirection = direction;
			
			if (IsOnFloor())
				selectAnimationForWalking();
		}
		else if (Input.IsActionPressed("crouch")) {
			if (IsOnFloor())
				animPlayer.Play("Crouch");
		}
		else if (Input.IsActionPressed("lookUp")) {
			if (IsOnFloor())
				animPlayer.Play("LookUp");
		}
		else {
			// If nothing is pressed then go to idle animation
			if (IsOnFloor())
				animPlayer.Play("Idle");
		}

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void selectAnimationForWalking() {
		// Crawl if player holds down while walking
		if (Input.IsActionPressed("crouch"))
			animPlayer.Play("Crawl");
		else
			animPlayer.Play("Walk");
	}

	// Handles the bounce physics when getting hurt
	public void tookDamage(double delta) {	
		Vector2 velocity = Velocity;

		// Player bounces up a little bit (2 times)
		if (IsOnFloor() && bounces < 2) {
			bounces++;
			velocity.Y = -205.0f;
		}

		// Player falls in the air
		if (!IsOnFloor()) {
			velocity.Y += gravity * (float)delta;
		}

		// Player bounces away from the enemy
		velocity.X = -lastDirection.X * Speed;

		Velocity = velocity;
		MoveAndSlide();

	}

	private async void OnAreaEntered(Area2D area) {
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

	private void handleInput() {
		if (Input.IsActionPressed("walkRight") || Input.IsActionPressed("walkLeft")) {
			holdingInput = true;
		}
		else if (!Input.IsActionPressed("walkRight") && !Input.IsActionPressed("walkLeft")) {
			holdingInput = false;
		}
	}

}

