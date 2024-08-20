using Godot;
using System;

public partial class Cursor : Node2D
{
	public int playerNum { get; set; }
	private Sprite2D sprite;
	private AudioComponent audio;
	private GameManager gm;
	private CharacterSelectScreen css;

	[Export] public int slot = 0;

	private Vector2 pos = new Vector2(0.0f, 0.0f);
	public bool characterSelected = false;

	private Vector2 goemonButton = new Vector2(660.0f, 250.0f);
	private Vector2 ebisumaruButton = new Vector2(960.0f, 250.0f);
	private Vector2 sasukeButton = new Vector2(1259.0f, 250.0f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		audio = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");
		css = GetNode<CharacterSelectScreen>("/root/Character Select Screen");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (characterSelected) {			
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				if (slot == 1) {
					CharacterDeselected("Ebisumaru");
				}
				else if (slot == 2) {
					CharacterDeselected("Sasuke");
				}
				else {
					CharacterDeselected("Goemon");
				}
			}

			if (Input.IsActionJustPressed("start")) {
				gm.DeleteCursors();
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
			if (slot == 0) {
				sprite.Position = goemonButton + pos;
			}
			else if (slot == 1) {
				sprite.Position = ebisumaruButton + pos;
			}
			else if (slot == 2) {
				sprite.Position = sasukeButton + pos;
			}

			// Move the cursor
			if (Input.IsActionJustPressed("walkLeft" + playerNum.ToString()) && slot > 0) {
				slot -= 1;
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}
			else if (Input.IsActionJustPressed("walkRight" + playerNum.ToString()) && slot < 2) {
				slot += 1;
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}

			// Player selects a character
			if (Input.IsActionJustPressed("jump" + playerNum.ToString())) {
				if (!css.buttons[slot].ButtonPressed) {
					if (slot == 1) {
						CharacterSelected("Ebisumaru");
					}
					else if (slot == 2) {
						CharacterSelected("Sasuke");
					}
					else {
						CharacterSelected("Goemon");
					}
				}
			}

			// Exit the CSS
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				css.BackButtonPressed();
			}
		}
	}

	private void CharacterSelected(string characterName) {
		audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);
		audio.playSFX("res://Sounds/SFX/" + characterName + "/selected.wav", -15.0f);
		gm.SetCharacter(playerNum, slot);

		// Button is pressed
		css.buttons[slot].ButtonPressed = true;

		// Disable the button from being pressed with mouse
		css.buttons[slot].TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/" + characterName + "ButtonPressed.png");
		css.buttons[slot].Disabled = true;

		Sprite2D sprite = GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		sprite.Frame = sprite.Frame + 3;

		characterSelected = true;
	}

	private void CharacterDeselected(string characterName) {
		if (playerNum == 0) {
			// Enable the buttons to be pressed with mouse
			css.EnableButtons();
		}
		else {
			css.buttons[slot].ButtonPressed = false;

			css.buttons[slot].TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/" + characterName + "Button.png");
			css.buttons[slot].Disabled = false;

			Sprite2D sprite = GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame - 3;

			characterSelected = false;	
		}
	}
}
