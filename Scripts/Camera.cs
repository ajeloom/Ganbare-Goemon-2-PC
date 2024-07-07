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
	private float[] posArray;

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
		// Stop moving camera when a player dies and is respawning
		if (!gm.IsPlayerRespawning()) {
			if (gm.playerNum > 1) {
				// Get average position of the players
				if (!gotAveragePos) {
					gotAveragePos = true;

					// Store the x-position of all the players in an array
					posArray = new float[3];
					for (int i = 0; i < 3; i++) {
						if (gm.players[i].isAlive)
							posArray[i] = gm.players[i].node.Position.X;
						else
							posArray[i] = Position.X;
					}

					// Sort array
					Array.Sort(posArray);

					// Find the average between the characters closest to each side of the screen
					posX = posArray[0] + posArray[gm.playerNum - 1];
					averageX = posX / 2;

					gotAveragePos = false;
				}
			}
			else if (gm.playerNum == 1) {
				averageX = gm.players[0].node.Position.X;
			}
			
			Position = new Vector2(averageX, 0.0f);
			leftSide.Position = new Vector2(Position.X - 335.0f, 120.0f);
			rightSide.Position = new Vector2(Position.X + 335.0f, 120.0f);
		}
	}
}
