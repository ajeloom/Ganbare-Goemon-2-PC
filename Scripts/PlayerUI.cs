using Godot;
using System;

public partial class PlayerUI : Node2D
{
	[Export] private Label coinsLabel;
	[Export] private Label livesLabel;
	[Export] Node2D node;

	private Player player;
	private HealthComponent healthComponent;

	private int playerNum;
	[Export] private TextureProgressBar bar1;
	[Export] private TextureProgressBar bar2;
	[Export] private TextureProgressBar bar3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Player>();
		

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player.coins < 10) {
			coinsLabel.Text = Convert.ToString("00" + player.coins);
		}
		else if (player.coins < 100) {
			coinsLabel.Text = Convert.ToString("0" + player.coins);
		}
		else if (player.coins < 1000) {
			coinsLabel.Text = Convert.ToString(player.coins);
		}

		if (player.lives < 10) {
			livesLabel.Text = Convert.ToString("0" + player.lives);
		}
		else if (player.lives < 100) {
			livesLabel.Text = Convert.ToString(player.lives);
		}

		bar1.Value = player.healthComponent.health;
		bar2.Value = player.healthComponent.health;
		bar3.Value = player.healthComponent.health;
	}
}
