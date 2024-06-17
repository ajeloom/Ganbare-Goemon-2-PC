using Godot;
using System;

public partial class Kabuki : CharacterBody2D
{
	public const float Speed = 300.0f;
	public float descendSpeed = 15.0f;
	public const float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool oneSec = true;
	private bool startUpAnimation = true;
	private bool startUpFall = true;
	private Vector2 targetPosition;

	private bool emerged = false;
	private bool descendSlowly = false;

	public bool landed = false;

	public bool shakeCamera = false;

	[Export] private AnimationPlayer animPlayer;

	// Different Attacks
	// Shoot flowers from basket then go in basket and fly a bit
	// Once basket breaks
	// Jump

	public override void _Ready()
	{
		Position = new Vector2(320.0f, 100.0f);

		targetPosition = new Vector2(320.0f, 140.0f);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Wait a second before moving
		if (oneSec) {
			startUp();
		}
		else {
			Vector2 velocity = Velocity;

			if (Position.Y >= 140.0f && startUpAnimation) {
				if (!emerged) {
					emerged = true;
					animPlayer.Play("emerge");
				}
			}
			
			if (!IsOnFloor()) {
				float currentGravity = (startUpAnimation) ? 35.0f : gravity;
				velocity.Y += currentGravity * (float)delta;
				landed = false;
			}

			// Start up animation ends when boss lands on the ground
			if (IsOnFloor()) {
				landed = true;
				startUpAnimation = false; // Gravity is normal
			}

			GD.Print(landed);

			// Handle Jump.
			if (IsOnFloor())
				velocity.Y = JumpVelocity;

			Velocity = velocity;
			MoveAndSlide();
		}	
	}

	private async void startUp() {
		// Wait one second
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		oneSec = false;
	}
}
