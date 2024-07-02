using Godot;
using System;

public partial class Impact : Node2D
{
	private GameManager gm;
	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private Node2D left;
	private Node2D right;
	private Sprite2D crosshair;

	private bool isLeftUsed = false;
	private bool gotMousePosition = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		left = GetNode<Node2D>("Left");
		right = GetNode<Node2D>("Right");

		crosshair = GetNode<Sprite2D>("Crosshair");
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Hide the mouse cursor if the game is not paused
		if (!gm.isPaused) {
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}

		// GD.Print(healthComponent.health);

		if (healthComponent.health > 0.0f) {
			if (!gotMousePosition) {
				gotMousePosition = true;
				
				

				Vector2 mousePos = GetGlobalMousePosition();
				if (mousePos.Y < -90.0f) {
					mousePos.Y = -90.0f;
				}
				if (mousePos.Y > 40.0f) {
					mousePos.Y = 40.0f;
				}
				if (mousePos.X < -180.0f) {
					mousePos.X = -180.0f;
				}
				if (mousePos.X > 180.0f) {
					mousePos.X = 180.0f;
				}
				crosshair.GlobalPosition = new Vector2(mousePos.X, mousePos.Y);
				gotMousePosition = false;
			}
			
			if (Input.IsActionJustPressed("leftPunch") && !animPlayer.IsPlaying()) {
				if (!isLeftUsed) {
					isLeftUsed = true;
					Vector2 mousePos = crosshair.GlobalPosition;

					// Limit the left arm to the left side
					if (mousePos.X > -15.0f) {
						mousePos.X = -15.0f;
					}
					if (mousePos.Y < -45.0f) {
						mousePos.Y = -45.0f;
					}

					left.GlobalPosition = new Vector2(mousePos.X - 85, mousePos.Y + 100);

					
					animPlayer.Play("leftPunch_2");
					isLeftUsed = false;
				}
				
			}
			else if (Input.IsActionJustPressed("rightPunch") && !animPlayer.IsPlaying()) {
				if (!isLeftUsed) {
					isLeftUsed = true;
					Vector2 mousePos = crosshair.GlobalPosition;

					// Limit the left arm to the left side
					if (mousePos.X < 15.0f) {
						mousePos.X = 15.0f;
					}
					if (mousePos.Y < -45.0f) {
						mousePos.Y = -45.0f;
					}

					right.GlobalPosition = new Vector2(mousePos.X + 85, mousePos.Y + 100);

					
					animPlayer.Play("rightPunch_2");
					isLeftUsed = false;
				}
				
			}
		}
		else {
			QueueFree();
		}
	}
}
