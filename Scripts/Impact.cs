using Godot;
using System;

public partial class Impact : Node2D
{
	private GameManager gm;
	private AnimationPlayer animPlayer;
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

		if (!gotMousePosition) {
			gotMousePosition = true;
			
			Vector2 mousePos = GetGlobalMousePosition();
			if (mousePos.Y < -60.0f) {
				mousePos.Y = -60.0f;
			}
			if (mousePos.Y > 40.0f) {
				mousePos.Y = 40.0f;
			}
			if (mousePos.X < -193.0f) {
				mousePos.X = -193.0f;
			}
			if (mousePos.X > 193.0f) {
				mousePos.X = 193.0f;
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

				
				animPlayer.Play("leftPunch");
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

				
				animPlayer.Play("rightPunch");
				isLeftUsed = false;
			}
			
		}
	}
}
