using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	private TextureButton button1, button2, button3;
	private GameManager gm;
	private AudioComponent audio;
	private Cursor cursor;

	public TextureButton[] buttons;

	public int mouseCurrentSlot = 0;

	private bool initialized = false;
	public bool hoveringButton = false;
	private bool checkButtons = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<TextureButton>("CanvasLayer/Goemon");
		button2 = GetNode<TextureButton>("CanvasLayer/Ebisumaru");
		button3 = GetNode<TextureButton>("CanvasLayer/Sasuke");
		gm = GetNode<GameManager>("/root/GameManager");
		audio = GetNode<AudioComponent>("AudioComponent");

		gm.selectCharacter = true;

		buttons = new TextureButton[3];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!initialized) {
			initialized = true;
			cursor = GetNode<Cursor>("/root/GameManager/Cursor");

			buttons[0] = button1;
			buttons[1] = button2;
			buttons[2] = button3;
		}

		if (cursor.characterSelected) {
			// Disable the buttons from being pressed
			DisableButtons();

			if (Input.IsActionJustPressed("back")) {
				EnableButtons();
				
				// Play sound when mouse is on another character slot
				if (hoveringButton) {
					audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
					cursor.slot = mouseCurrentSlot;
				}
			}
		}
	}

	public void GoemonButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 0;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Goemon/selected.wav", -15.0f);
			gm.setCharacter(0, 0);

			button1.ButtonPressed = true;

			// Disable the other buttons from being pressed
			button1.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/GoemonButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;
		}
	}

	private void GoemonMouseEntered() {
		if (!cursor.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);

			cursor.slot = 0;
		}
		hoveringButton = true;
		mouseCurrentSlot = 0;
	}

	public void EbisumaruButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 1;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Ebisumaru/selected.wav", -15.0f);
			gm.setCharacter(0, 1);

			button2.ButtonPressed = true;

			button2.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/EbisumaruButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;
		}
	}

	private void EbisumaruMouseEntered() {
		if (!cursor.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);

			cursor.slot = 1;
		}
		hoveringButton = true;
		mouseCurrentSlot = 1;
	}

	public void SasukeButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 2;

			audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);

			audio.playSFX("res://Sounds/SFX/Sasuke/selected.wav", -15.0f);
			gm.setCharacter(0, 2);

			button3.ButtonPressed = true;

			button3.TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/SasukeButtonPressed.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;
		}
	}

	private void SasukeMouseEntered() {
		if (!cursor.characterSelected) {
			audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);

			cursor.slot = 2;
		}
		hoveringButton = true;
		mouseCurrentSlot = 2;
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
		// Switch texture of the button to the unselected version
		if (cursor.slot == 0) {
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/GoemonButton.png");
		}
		else if (cursor.slot == 1) {
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/EbisumaruButton.png");
		}
		else if (cursor.slot == 2) {
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load("res://Sprites/UI/Menu/SasukeButton.png");
		}		
	
		// Make the current button unpressed
		buttons[cursor.slot].ButtonPressed = false;

		// Allows the buttons to be pushed again
		if (!checkButtons) {
			checkButtons = true;
			for (int i = 0; i < 3; i++) {
				// If button is not pressed then enable the button to be pressed
				if (!buttons[i].ButtonPressed) {
					buttons[i].Disabled = false;
				}
			}
			checkButtons = false;
		}

		// Change the cursor the arrow sprite
		Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		sprite.Frame = sprite.Frame - 3;

		cursor.characterSelected = false;
	}

	private void MouseExitedButton() {
		hoveringButton = false;
	}
}
