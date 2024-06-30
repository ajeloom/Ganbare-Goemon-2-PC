using Godot;
using System;

public partial class Senshuraku : CharacterBody2D
{
	[Export] public float speed = 100.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 targetPosition;

	private bool goToPos = false;

	// Attack variables
	private bool shootBallAttack = false;

	public override void _Ready() {
		Position = new Vector2(0.0f, 10.0f);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (Position.Y < 7.0f) {
			Position = new Vector2(Position.X, 7.0f);
		}
		else if (Position.Y > 40.0f) {
			Position = new Vector2(Position.X, 40.0f);
		}

		Scale = new Vector2(0.1f * Position.Y, 0.1f * Position.Y);

		// if (shootBallAttack) {
		// 	if (!goToPos) {
		// 		goToPos = true;
		// 		targetPosition = new Vector2(-117.0f, 10.0f);
		// 		var direction = GlobalPosition.DirectionTo(targetPosition);
		// 		if (Position != targetPosition)
		// 			velocity = direction * speed;
		// 		goToPos = false;
		// 	}
			
		// }
		

		// Add the gravity.
		// if (!IsOnFloor())
		// 	velocity.Y += gravity * (float)delta;

		if (Input.IsKeyPressed(Key.Up)
			|| Input.IsKeyPressed(Key.Down)
			|| Input.IsKeyPressed(Key.Left)
			|| Input.IsKeyPressed(Key.Right)) {
			if (Input.IsKeyPressed(Key.Up)) {
				velocity.Y = -50.0f;
			}
			if (Input.IsKeyPressed(Key.Down)) {
				velocity.Y = 50.0f;
			}
			if (Input.IsKeyPressed(Key.Left)) {
				velocity.X = -speed * 0.15f * Position.Y;
			}
			if (Input.IsKeyPressed(Key.Right)) {
				velocity.X = speed * 0.15f * Position.Y;
			}
		}
		else {
			velocity = Vector2.Zero;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ShootBallAttack() {
		

	}

	private float GetRandomPosition(int length) {
		Random rnd = new Random();
		int num = rnd.Next(length);
		return num; 
	}
}
