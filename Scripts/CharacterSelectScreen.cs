using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	private TextureButton button1, button2, button3;
	private Button nextButton, cancelButton;
	private Sprite2D background;
	private GameManager gm;
	private AudioComponent audioComponent;
	private Cursor cursor;

	public TextureButton[] buttons;

	public int mouseCurrentSlot = 0;

	private bool initialized = false;
	public bool hoveringButton = false;
	private bool checkButtons = false;
	private bool characterSelected = false;

	private string spritePath = "res://Sprites/UI/Menu/CharacterSelect/";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<TextureButton>("CanvasLayer/GoemonButton");
		button2 = GetNode<TextureButton>("CanvasLayer/EbisumaruButton");
		button3 = GetNode<TextureButton>("CanvasLayer/SasukeButton");
		nextButton = GetNode<Button>("CanvasLayer/NextButton");
		cancelButton = GetNode<Button>("CanvasLayer/CancelButton");
		background = GetNode<Sprite2D>("CanvasLayer/Background");
		gm = GetNode<GameManager>("/root/GameManager");
		audioComponent = GetNode<AudioComponent>("AudioComponent");

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

			// if (Input.IsActionJustPressed("back")) {
			// 	CancelButtonPressed();
			// }
		}

		if (!IsCharacterSelected()) {
			nextButton.Visible = false;
			cancelButton.Visible = false;
		}
		else {
			nextButton.Visible = true;
			cancelButton.Visible = true;
		}
	}

	public void GoemonButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 0;

			PlayButtonClickedSFX();

			audioComponent.playSFX("res://Sounds/SFX/Goemon/selected.wav", -15.0f);
			gm.SetCharacter(0, 0);

			button1.ButtonPressed = true;

			// Disable the other buttons from being pressed
			button1.TextureDisabled = (Texture2D)GD.Load(spritePath + "GoemonMenuHover.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect1.png");
		}
	}

	private void GoemonMouseEntered() {
		if (!cursor.characterSelected) {
			ButtonMouseEntered();
			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect1.png");

			cursor.slot = 0;
		}
		hoveringButton = true;
		mouseCurrentSlot = 0;
	}

	public void EbisumaruButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 1;

			PlayButtonClickedSFX();

			audioComponent.playSFX("res://Sounds/SFX/Ebisumaru/selected.wav", -15.0f);
			gm.SetCharacter(0, 1);

			button2.ButtonPressed = true;

			button2.TextureDisabled = (Texture2D)GD.Load(spritePath + "EbisumaruMenuHover.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect2.png");
		}
	}

	private void EbisumaruMouseEntered() {
		if (!cursor.characterSelected) {
			ButtonMouseEntered();
			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect2.png");

			cursor.slot = 1;
		}
		hoveringButton = true;
		mouseCurrentSlot = 1;
	}

	public void SasukeButtonPressed() {
		if (!cursor.characterSelected) {
			cursor.characterSelected = true;

			cursor.slot = 2;

			PlayButtonClickedSFX();

			audioComponent.playSFX("res://Sounds/SFX/Sasuke/selected.wav", -15.0f);
			gm.SetCharacter(0, 2);

			button3.ButtonPressed = true;

			button3.TextureDisabled = (Texture2D)GD.Load(spritePath + "SasukeMenuHover.png");
			DisableButtons();

			Sprite2D sprite = cursor.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
			sprite.Frame = sprite.Frame + 3;

			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect3.png");
		}
	}

	private void SasukeMouseEntered() {
		if (!cursor.characterSelected) {
			ButtonMouseEntered();
			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect3.png");

			cursor.slot = 2;
		}
		hoveringButton = true;
		mouseCurrentSlot = 2;
	}

	private void NextButtonPressed() {
		if (IsCharacterSelected()) {
			PlayButtonClickedSFX();
			gm.DeleteCursors();
			gm.selectCharacter = false;
			gm.Transition("res://Scenes/StageSelectScreen.tscn");
		}
	}

	public void BackButtonPressed() {
		PlayButtonClickedSFX();
		gm.DeleteCursors();
		gm.selectCharacter = false;
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
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load(spritePath + "GoemonMenu.png");
		}
		else if (cursor.slot == 1) {
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load(spritePath + "EbisumaruMenu.png");
		}
		else if (cursor.slot == 2) {
			buttons[cursor.slot].TextureDisabled = (Texture2D)GD.Load(spritePath + "SasukeMenu.png");
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

		if (!button1.ButtonPressed && !button2.ButtonPressed && !button3.ButtonPressed) {
			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect.png");
		}
	}

	private bool IsCharacterSelected() {
		if (button1.ButtonPressed || button2.ButtonPressed || button3.ButtonPressed) {
			return true;
		}

		return false;
	}

	private void CancelButtonPressed() {
		EnableButtons();

		// Play sound when mouse is on another character slot
		if (hoveringButton) {
			ButtonMouseEntered();
			cursor.slot = mouseCurrentSlot;
		}
		else {
			background.Texture = (Texture2D)GD.Load(spritePath + "CharacterSelect.png");
		}
	
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;
	}

	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX() {
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}
}
