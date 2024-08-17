using Godot;
using System;

public partial class Cursor : Node2D
{
	public int playerNum { get; set; }
	private Sprite2D sprite;
	private AudioComponent audio;
	private GameManager gm;
	private CharacterSelectScreen css;

	[Export] public int slot = 1;

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
		css = GetNode<CharacterSelectScreen>("/root/Character Select Screen");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (gm.characterSelected) {			
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				css.EnableButtons();
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
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}
			else if (Input.IsActionJustPressed("walkRight" + playerNum.ToString()) && slot < 3) {
				slot += 1;
				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}

			// Player selects a character
			if (Input.IsActionJustPressed("jump" + playerNum.ToString())) {
				if (slot == 2) {
					css.EbisumaruButtonPressed();
					gm.setCharacter(playerNum, 1);
				}
				else if (slot == 3) {
					css.SasukeButtonPressed();
					gm.setCharacter(playerNum, 2);
				}
				else {
					css.GoemonButtonPressed();
					gm.setCharacter(playerNum, 0);
				}
			}

			// Exit the CSS
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				css.BackButtonPressed();
			}
		}
	}
}
