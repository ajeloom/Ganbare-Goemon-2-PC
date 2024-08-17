using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	private TextureButton button1, button2, button3;
	private GameManager gm;
	private AudioComponent audio;
	private Cursor cursor;

	private int lastSelected = 0;

	private bool initialized = false;
	private bool mouseEntered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<TextureButton>("CanvasLayer/Goemon");
		button2 = GetNode<TextureButton>("CanvasLayer/Ebisumaru");
		button3 = GetNode<TextureButton>("CanvasLayer/Sasuke");
		gm = GetNode<GameManager>("/root/GameManager");
		audio = GetNode<AudioComponent>("AudioComponent");

		gm.selectCharacter = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!initialized) {
			initialized = true;
			cursor = GetNode<Cursor>("/root/GameManager/Cursor");
		}

		if (gm.characterSelected) {
			if (Input.IsActionJustPressed("back")) {
				EnableButtons();
				
				// Play sound when mouse is on another character slot
				if (mouseEntered) {
					audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
				}
			}
		}
	}

	public void GoemonButtonPressed() {
		if (!gm.characterSelected) {
			gm.characterSelected = true;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Goemon/selected.wav", -15.0f);
			gm.setCharacter(0, 0);

			button1.ButtonPressed = true;
			button2.ButtonPressed = false;
			button3.ButtonPressed = false;

			// Disable the other buttons from being pressed
			button1.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/GoeButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			lastSelected = 1;
		}
	}

	private void GoemonMouseEntered() {
		if (!gm.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
		}
		mouseEntered = true;
		cursor.slot = 1;
	}

	public void EbisumaruButtonPressed() {
		if (!gm.characterSelected) {
			gm.characterSelected = true;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Ebisumaru/selected.wav", -15.0f);
			gm.setCharacter(0, 1);

			button1.ButtonPressed = false;
			button2.ButtonPressed = true;
			button3.ButtonPressed = false;

			button2.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/EbiButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			lastSelected = 2;
		}
	}

	private void EbisumaruMouseEntered() {
		if (!gm.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
		}
		mouseEntered = true;
		cursor.slot = 2;
	}

	public void SasukeButtonPressed() {
		if (!gm.characterSelected) {
			gm.characterSelected = true;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Sasuke/selected.wav", -15.0f);
			gm.setCharacter(0, 2);

			button1.ButtonPressed = false;
			button2.ButtonPressed = false;
			button3.ButtonPressed = true;

			button3.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/SasButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			lastSelected = 3;
		}
	}

	private void SasukeMouseEntered() {
		if (!gm.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
		}
		mouseEntered = true;
		cursor.slot = 3;
	}

	private void ContinueButtonPressed() {
		if (button1.ButtonPressed || button2.ButtonPressed || button3.ButtonPressed) {
			gm.deleteCursors();
			gm.selectCharacter = false;
			gm.Transition("res://Scenes/StageSelectScreen.tscn");
		}
	}

	public void BackButtonPressed() {
		gm.selectCharacter = false;
		gm.characterSelected = false;
		gm.Transition("res://Scenes/PlayerNumberScreen.tscn");
	}

	// This function disables the buttons from
	// being pressed when a character is picked
	public void DisableButtons() {
		button1.Disabled = true;
		button2.Disabled = true;
		button3.Disabled = true;
	}

	// This function is used when player deselects their character
	public void EnableButtons() {
		// Switch texture to all the buttons to the unselected image
		button1.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/GoeButton.png");
		button2.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/EbiButton.png");
		button3.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/SasButton.png");

		// Make sure no buttons are pressed
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;

		// Allows the buttons to be pushed again
		button1.Disabled = false;
		button2.Disabled = false;
		button3.Disabled = false;

		// Change the cursor the arrow sprite
		Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		sprite.Frame = sprite.Frame - 3;

		gm.characterSelected = false;
	}

	private void MouseExitedButton() {
		mouseEntered = false;
		cursor.slot = lastSelected;
	}
}
