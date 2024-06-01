using Godot;
using System;

public partial class Goemon : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -350.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export] private Sprite2D sprite;
	[Export] private AnimationTree animTree;

	public override void _Ready()
	{
		sprite.FlipH = false;
		setParametersToFalse();
		animTree.Set("parameters/conditions/isIdle", true);
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
				setParametersToFalse();
				animTree.Set("parameters/conditions/isJumping", true);
			}
			else {
				setParametersToFalse();
				animTree.Set("parameters/conditions/isFalling", true);
			}
			
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.Y = JumpVelocity;

			setParametersToFalse();
			animTree.Set("parameters/conditions/isJumping", true);
		}
			

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Vector2.Zero;
		if (Input.IsActionPressed("walkRight")) {
			sprite.FlipH = false;
			direction = new Vector2(1.0f, 0.0f);

			if (IsOnFloor()) {
				selectAnimationForWalking();
			}
		}
		else if (Input.IsActionPressed("walkLeft")) {
			sprite.FlipH = true;
			direction = new Vector2(-1.0f, 0.0f);
			
			if (IsOnFloor()) {
				selectAnimationForWalking();
			}
		}
		else {
			// If nothing is pressed then go to idle animation
			if (IsOnFloor()) {
				setParametersToFalse();
				animTree.Set("parameters/conditions/isIdle", true);
			}
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

	void setParametersToFalse() {
		animTree.Set("parameters/conditions/isIdle", false);
		animTree.Set("parameters/conditions/isWalking", false);
		animTree.Set("parameters/conditions/isCrawling", false);
		animTree.Set("parameters/conditions/isJumping", false);
		animTree.Set("parameters/conditions/isFalling", false);
	}

	void selectAnimationForWalking() {
		// Crawl if player holds down while walking
		if (Input.IsActionPressed("crouch")) {
			setParametersToFalse();
			animTree.Set("parameters/conditions/isCrawling", true);
		}
		else {
			setParametersToFalse();
			animTree.Set("parameters/conditions/isWalking", true);
		}
	}

}

