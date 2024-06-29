using Godot;
using System;

public partial class Camera : Camera2D
{
	private CharacterBody2D leftSide;
	private CharacterBody2D rightSide;
	private GameManager gm;

	private float posX = 0.0f;
	private float averageX = 0.0f;
	private bool gotAveragePos = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");

		leftSide = gm.GetNode<CharacterBody2D>("Boundary");
		rightSide = gm.GetNode<CharacterBody2D>("Boundary2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Get the average
		if (!gotAveragePos) {
			gotAveragePos = true;

			posX = 0.0f;
			for (int i = 0; i < gm.playerNum; i++) {
				posX += gm.players[i].node.Position.X;
			}
			averageX = posX / gm.playerNum;

			gotAveragePos = false;
		}

		Position = new Vector2(averageX, 0.0f);
		leftSide.Position = new Vector2(Position.X - 335.0f, 120.0f);
		rightSide.Position = new Vector2(Position.X + 335.0f, 120.0f);
	}
}
