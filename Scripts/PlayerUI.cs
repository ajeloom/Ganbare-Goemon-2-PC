using Godot;
using System;

public partial class PlayerUI : Node2D
{
	[Export] private Label coinsLabel;
	[Export] private Label livesLabel;
	[Export] Node2D node;

	private Player player;
	private HealthComponent healthComponent;

	[Export] private TextureProgressBar bar1;
	[Export] private TextureProgressBar bar2;
	[Export] private TextureProgressBar bar3;

	private bool gotPosition = false;

	private GameManager gm;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");
		player = GetParent<Player>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!gotPosition) {
			gotPosition = true;
			if (player.playerNum == 1) {
				node.Position = new Vector2(0.0f, 0.0f);
			}
			else if (player.playerNum == 2) {
				node.Position = new Vector2(575.0f, 0.0f);
			}
			else if (player.playerNum == 3) {
				node.Position = new Vector2(1150.0f, 0.0f);
			}
		}
		
		if (player.coins < 10) {
			coinsLabel.Text = Convert.ToString("00" + player.coins);
		}
		else if (player.coins < 100) {
			coinsLabel.Text = Convert.ToString("0" + player.coins);
		}
		else if (player.coins < 1000) {
			coinsLabel.Text = Convert.ToString(player.coins);
		}

		int lives = gm.players[player.playerNum - 1].lives;
		if (lives < 10) {
			livesLabel.Text = Convert.ToString("0" + lives);
		}
		else if (lives < 100) {
			livesLabel.Text = Convert.ToString(lives);
		}

		bar1.Value = player.healthComponent.health;
		bar2.Value = player.healthComponent.health;
		bar3.Value = player.healthComponent.health;
	}
}
