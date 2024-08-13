using Godot;
using System;

public partial class Cursor : Node2D
{
	public int playerNum { get; set; }
	private Sprite2D sprite;
	private AudioComponent audio;
	private GameManager gm;

	[Export] private int slot = 1;

	private bool isInputPressed = false;
	private bool characterSelected = false;

	private Vector2 pos = new Vector2(0.0f, 0.0f);

	private Vector2 goemonButton = new Vector2(660.0f, 250.0f);
	private Vector2 ebisumaruButton = new Vector2(960.0f, 250.0f);
	private Vector2 sasukeButton = new Vector2(1259.0f, 250.0f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		audio = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (characterSelected) {			
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				TextureButton button;
				if (slot == 2) {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Ebisumaru");
				}
				else if (slot == 3) {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Sasuke");
				}
				else {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Goemon");
				}

				button.ButtonPressed = false;
				sprite.Frame = sprite.Frame - 3;
				characterSelected = false;
			}

			if (Input.IsActionJustPressed("start")) {
				gm.deleteCursors();
				gm.selectCharacter = false;
				gm.GoToScene("res://Scenes/StageSelectScreen.tscn");
			}
		}
		else {
			// Position the cursor based on player
			if (gm.playerNum == 3) {
				if (playerNum == 0) {
					pos = new Vector2(-64.0f, 0.0f);
				}
				else if (playerNum == 2) {
					pos = new Vector2(64.0f, 0.0f);
				}
			}
			else if (gm.playerNum == 2) {
				if (playerNum == 0) {
					pos = new Vector2(-32.0f, 0.0f);
				}
				else if (playerNum == 1) {
					pos = new Vector2(32.0f, 0.0f);
				}
			}
			
			// Position the cursor
			if (slot == 1) {
				sprite.Position = goemonButton + pos;
			}
			else if (slot == 2) {
				sprite.Position = ebisumaruButton + pos;
			}
			else if (slot == 3) {
				sprite.Position = sasukeButton + pos;
			}

			// Move the cursor
			if (Input.IsActionJustPressed("walkLeft" + playerNum.ToString()) && slot > 1) {
				slot -= 1;
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -10.0f);
			}
			else if (Input.IsActionJustPressed("walkRight" + playerNum.ToString()) && slot < 3) {
				slot += 1;
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -10.0f);
			}

			// Player selects a character
			if (Input.IsActionJustPressed("jump" + playerNum.ToString())) {
				audio.playSFX("res://Sounds/SFX/menuClick.wav", 0.0f);

				TextureButton button;
				if (slot == 2) {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Ebisumaru");
					audio.playSFX("res://Sounds/SFX/Ebisumaru/selected.wav", -10.0f);
					gm.setCharacter(playerNum, 1);
				}
				else if (slot == 3) {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Sasuke");
					audio.playSFX("res://Sounds/SFX/Sasuke/selected.wav", -10.0f);
					gm.setCharacter(playerNum, 2);
				}
				else {
					button = GetNode<TextureButton>("/root/Character Select Screen/CanvasLayer/Goemon");
					audio.playSFX("res://Sounds/SFX/Goemon/selected.wav", -10.0f);
					gm.setCharacter(playerNum, 0);
				}

				button.ButtonPressed = true;
				sprite.Frame = sprite.Frame + 3;
				characterSelected = true;
			}

			// Exit the CSS
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				gm.selectCharacter = false;
				gm.GoToScene("res://Scenes/PlayerNumberScreen.tscn");
			}
		}
	}
}
