using Godot;
using System;

public partial class Camera : Camera2D
{
	private CharacterBody2D leftSide;
	private CharacterBody2D rightSide;

	private float posX = 0.0f;
	private float averageX = 0.0f;
	private bool gotAveragePos = false;
	private float[] posArray;

	private Player player1;

	[Export] private float offset = 75.0f;
	[Export] private float camSpeed = 5.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GameManager.instance.playerCount > 1)
		{
			leftSide = GameManager.instance.GetNode<CharacterBody2D>("Boundary");
			rightSide = GameManager.instance.GetNode<CharacterBody2D>("Boundary2");
		}

		player1 = GameManager.instance.players[(int)GameManager.PlayerNumber.Player1].node;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Stop moving camera when a player dies and is respawning
		if (!GameManager.instance.IsPlayerRespawning()) {
			if (GameManager.instance.playerCount > 1) {
				// Get average position of the players
				if (!gotAveragePos) {
					gotAveragePos = true;

					// Store the x-position of all the players in an array
					posArray = new float[GameManager.instance.playerCount];
					for (int i = 0; i < GameManager.instance.playerCount; i++) {
						if (GameManager.instance.players[i].isAlive)
							posArray[i] = GameManager.instance.players[i].node.Position.X;
						else
							posArray[i] = Position.X;
					}

					// Sort array
					Array.Sort(posArray);

					// Find the average between the characters closest to each side of the screen
					posX = posArray[0] + posArray[GameManager.instance.playerCount - 1];
					averageX = posX / 2;

					gotAveragePos = false;
				}
			}
			else if (GameManager.instance.playerCount == 1) {
				averageX = player1.Position.X;
				
				// Experimental camera that tries to mimic the one from the original game
				// float direction = player1.Velocity.Normalized().X;
				// if (direction != 0.0f) {
				// 	averageX = player1.Position.X + (direction * offset);
				// }
				
				// Position = Position.Lerp(new Vector2(averageX, 0.0f), (float)delta * camSpeed);
			}
			
			Position = new Vector2(averageX, 0.0f);
			if (GameManager.instance.playerCount > 1) {
				leftSide.Position = new Vector2(Position.X - 335.0f, 120.0f);
				rightSide.Position = new Vector2(Position.X + 335.0f, 120.0f);
			}
		}
	}
}
