using Godot;
using System;

public partial class Goemon : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -350.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D sprite;
	[Export] private AnimationPlayer animPlayer;

	public override void _Ready()
	{
		sprite.FlipH = false;	
		animPlayer.Play("Idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		//GD.Print(velocity);

		//GD.Print(IsOnFloor());

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

			if (IsOnFloor())
				selectAnimationForWalking();
		}
		else if (Input.IsActionPressed("walkLeft")) {
			sprite.FlipH = true;
			direction = new Vector2(-1.0f, 0.0f);
			
			if (IsOnFloor())
				selectAnimationForWalking();
		}
		else if (Input.IsActionPressed("lookUp")) {
			if (IsOnFloor())
				animPlayer.Play("LookUp");
		}
		else if (Input.IsActionPressed("crouch")) {
			if (IsOnFloor())
				animPlayer.Play("Crouch");
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

	void selectAnimationForWalking() {
		// Crawl if player holds down while walking
		if (Input.IsActionPressed("crouch"))
			animPlayer.Play("Crawl");
		else
			animPlayer.Play("Walk");
	}

}

