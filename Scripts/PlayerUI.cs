using Godot;
using System;

public partial class PlayerUI : Node2D
{
	// UI variables
	private CanvasLayer canvas;
	private Label coinsLabel;
	private Label livesLabel;
	private Sprite2D weaponIcon;
	private Sprite2D charaIcon;
	private TextureProgressBar bar1;
	private TextureProgressBar bar2;
	private TextureProgressBar bar3;

	private Player player;

	private bool initialized = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Player>();

		canvas = GetNode<CanvasLayer>("CanvasLayer");

		coinsLabel = GetNode<Label>("CanvasLayer/Coins Label");
		livesLabel = GetNode<Label>("CanvasLayer/Lives Label");

		weaponIcon = GetNode<Sprite2D>("CanvasLayer/Weapon Icon");
		charaIcon = GetNode<Sprite2D>("CanvasLayer/Icon");

		bar1 = GetNode<TextureProgressBar>("CanvasLayer/HP Bar 1");
		bar2 = GetNode<TextureProgressBar>("CanvasLayer/HP Bar 2");
		bar3 = GetNode<TextureProgressBar>("CanvasLayer/HP Bar 3");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void InitializeUI(int playerNum, int character)
	{
		// Set the position of the UI based on player number
		if (playerNum == (int)GameManager.PlayerNumber.Player1) {
			canvas.Offset = new Vector2(0.0f, 0.0f);
		}
		else if (playerNum == (int)GameManager.PlayerNumber.Player2) {
			canvas.Offset = new Vector2(73.0f, 0.0f);
		}
		else if (playerNum == (int)GameManager.PlayerNumber.Player3) {
			canvas.Offset = new Vector2(146.0f, 0.0f);
		}

		// Set the matching weapon icon
		if (character == (int)Player.CharacterID.Goemon)
			weaponIcon.Texture = (Texture2D)GD.Load("res://Sprites/UI/Player UI/Weapon1Icon.png");
		else if (character == (int)Player.CharacterID.Ebisumaru)
			weaponIcon.Texture = (Texture2D)GD.Load("res://Sprites/UI/Player UI/Fan1Icon.png");
		else if (character == (int)Player.CharacterID.Sasuke)
			weaponIcon.Texture = (Texture2D)GD.Load("res://Sprites/UI/Player UI/Kunai1Icon.png");

		// Set the matching icon to the character
		charaIcon.Frame = character;
	}

	public void UpdateCoins()
	{
		// Display the coins
		int coins = GameManager.instance.players[player.playerNum].coins;
		if (coins < 10) {
			coinsLabel.Text = Convert.ToString("00" + coins);
		}
		else if (coins < 100) {
			coinsLabel.Text = Convert.ToString("0" + coins);
		}
		else if (coins < 1000) {
			coinsLabel.Text = Convert.ToString(coins);
		}
	}

	public void UpdateLives()
	{
		// Display the lives
		int lives = player.lives;
		if (lives == -1) {
			livesLabel.Text = Convert.ToString("00");
		}
		else if (lives < 10) {
			livesLabel.Text = Convert.ToString("0" + lives);
		}
		else if (lives < 100) {
			livesLabel.Text = Convert.ToString(lives);
		}
	}

	public void UpdateHealth()
	{
		// Update the health bar UI
		bar1.Value = player.healthComponent.health;
		bar2.Value = player.healthComponent.health;
		bar3.Value = player.healthComponent.health;
	}
}
