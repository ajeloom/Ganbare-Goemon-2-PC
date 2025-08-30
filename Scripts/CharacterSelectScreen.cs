using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	private TextureButton goemonButton, ebisumaruButton, sasukeButton;
	private Button nextButton, cancelButton;
	private Sprite2D background;
	private AudioComponent audioComponent;
	private Cursor cursor, cursor2, cursor3;

	public TextureButton[] buttons;
	public Cursor[] cursors;
	public int[] slots;

	private bool initialized = false;
	private bool checkButtons = false;
	private bool characterSelected = false;
	private bool checkingCursors = false;

	private string spritePath = "res://Sprites/UI/Menu/CharacterSelect/";

	public Sprite2D lightLeft, lightMiddle, lightRight;

	private Node2D cursorPosition1, cursorPosition2, cursorPosition3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		goemonButton = GetNode<TextureButton>("CanvasLayer/GoemonButton");
		ebisumaruButton = GetNode<TextureButton>("CanvasLayer/EbisumaruButton");
		sasukeButton = GetNode<TextureButton>("CanvasLayer/SasukeButton");
		nextButton = GetNode<Button>("CanvasLayer/NextButton");
		cancelButton = GetNode<Button>("CanvasLayer/CancelButton");
		background = GetNode<Sprite2D>("CanvasLayer/Background");
		audioComponent = GetNode<AudioComponent>("AudioComponent");

		lightLeft = GetNode<Sprite2D>("CanvasLayer/LightLeft");
		lightMiddle = GetNode<Sprite2D>("CanvasLayer/LightMiddle");
		lightRight = GetNode<Sprite2D>("CanvasLayer/LightRight");

		cursorPosition1 = GetNode<Node2D>("CanvasLayer/CursorPosition1");
		cursorPosition2 = GetNode<Node2D>("CanvasLayer/CursorPosition2");
		cursorPosition3 = GetNode<Node2D>("CanvasLayer/CursorPosition3");

		GameManager.instance.selectCharacter = true;

		cancelButton.Visible = false;
		nextButton.Visible = false;

		buttons = new TextureButton[3];
		cursors = new Cursor[3];
		slots = new int[3];

		slots[0] = 0;
		slots[1] = 0;
		slots[2] = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!initialized) {
			initialized = true;
			cursor = GetNodeOrNull<Cursor>("/root/GameManager/Cursor");
			cursor2 = GetNodeOrNull<Cursor>("/root/GameManager/Cursor2");
			cursor3 = GetNodeOrNull<Cursor>("/root/GameManager/Cursor3");

			cursors[(int)GameManager.PlayerNumber.Player1] = cursor;
			cursors[(int)GameManager.PlayerNumber.Player2] = cursor2;
			cursors[(int)GameManager.PlayerNumber.Player3] = cursor3;

			buttons[(int)Player.CharacterID.Ebisumaru] = ebisumaruButton;
			buttons[(int)Player.CharacterID.Goemon] = goemonButton;
			buttons[(int)Player.CharacterID.Sasuke] = sasukeButton;
		}

		CheckCursors();

		// Show next button when every player has picked a character
		if (!IsEveryPlayerReady()) {
			nextButton.Visible = false;
		}
		else {
			nextButton.Visible = true;

			if (Input.IsActionJustPressed("start")) {
				NextButtonPressed();
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
	}

	public void GoemonButtonPressed()
	{
		cursor.SelectedCharacter("Goemon");
	}

	private void GoemonMouseEntered()
	{
		if (!cursor.characterSelected
				&& !buttons[(int)Player.CharacterID.Goemon].ButtonPressed) {
			PlayMovedCursorSFX();
			cursor.slot = (int)Player.CharacterID.Goemon;
		}
	}

	public void EbisumaruButtonPressed()
	{
		cursor.SelectedCharacter("Ebisumaru");
	}

	private void EbisumaruMouseEntered()
	{
		if (!cursor.characterSelected
				&& !buttons[(int)Player.CharacterID.Ebisumaru].ButtonPressed) {
			PlayMovedCursorSFX();
			cursor.slot = (int)Player.CharacterID.Ebisumaru;
		}
	}

	public void SasukeButtonPressed()
	{
		cursor.SelectedCharacter("Sasuke");
	}

	private void SasukeMouseEntered()
	{
		if (!cursor.characterSelected
				&& !buttons[(int)Player.CharacterID.Sasuke].ButtonPressed) {
			PlayMovedCursorSFX();
			cursor.slot = (int)Player.CharacterID.Sasuke;
		}
	}

	private void NextButtonPressed()
	{
		PlayButtonClickedSFX();
		GameManager.instance.DeleteCursors();
		GameManager.instance.selectCharacter = false;
		GameManager.instance.Transition("res://Scenes/StageSelectScreen.tscn");
	}

	private void CancelButtonPressed()
	{
		cursor.DeselectedCharacter();

		EnableButtons();
		cancelButton.Visible = false;
	}

	public void ShowCancelButton(bool value)
	{
		cancelButton.Visible = value;
	}

	public void BackButtonPressed()
	{
		PlayButtonClickedSFX();
		GameManager.instance.DeleteCursors();
		GameManager.instance.selectCharacter = false;
		GameManager.instance.Transition("res://Scenes/PlayerNumberScreen.tscn");
	}

	// This function disables the buttons from
	// being pressed when a character is picked
	public void DisableButtons()
	{
		goemonButton.Disabled = true;
		ebisumaruButton.Disabled = true;
		sasukeButton.Disabled = true;
	}

	// This function is used when player deselects their character
	public void EnableButtons()
	{
		// Make the current button unpressed
		buttons[cursor.slot].ButtonPressed = false;

		// Allows the buttons to be pushed again
		if (!checkButtons)
		{
			checkButtons = true;
			for (int i = 0; i < 3; i++)
			{
				// If button is not pressed then enable the button to be pressed
				if (!buttons[i].ButtonPressed)
				{
					buttons[i].Disabled = false;
				}
			}
			checkButtons = false;
		}

		cursor.characterSelected = false;
	}

	private bool IsEveryPlayerReady()
	{
		for (int i = 0; i < GameManager.instance.playerCount; i++)
		{
			if (!cursors[i].characterSelected)
			{
				return false;
			}
		}

		return true;
	}

	private void PlayMovedCursorSFX()
	{
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX()
	{
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}

	// Update which characters a light is hitting
	private void CheckCursors()
	{
		if (!checkingCursors) {
			checkingCursors = true;

			// Reset each slot count
			slots[(int)Player.CharacterID.Goemon] = 0;
			slots[(int)Player.CharacterID.Ebisumaru] = 0;
			slots[(int)Player.CharacterID.Sasuke] = 0;

			// Count how many cursors on each slot
			for (int i = 0; i < GameManager.instance.playerCount; i++) {
				Cursor temp = cursors[i];

				if (temp.slot == (int)Player.CharacterID.Goemon) {
					lightMiddle.Visible = true;
					goemonButton.TextureNormal = (Texture2D)GD.Load(spritePath + "GoemonMenuHover.png");
					slots[(int)Player.CharacterID.Goemon]++;
				}
				else if (temp.slot == (int)Player.CharacterID.Ebisumaru) {
					lightLeft.Visible = true;
					ebisumaruButton.TextureNormal = (Texture2D)GD.Load(spritePath + "EbisumaruMenuHover.png");
					slots[(int)Player.CharacterID.Ebisumaru]++;
				}
				else if (temp.slot == (int)Player.CharacterID.Sasuke) {
					lightRight.Visible = true;
					sasukeButton.TextureNormal = (Texture2D)GD.Load(spritePath + "SasukeMenuHover.png");
					slots[(int)Player.CharacterID.Sasuke]++;
				}
			}

			// Remove light on character if no cursor is on them
			if (slots[(int)Player.CharacterID.Goemon] == 0) {
				lightMiddle.Visible = false;
				goemonButton.TextureNormal = (Texture2D)GD.Load(spritePath + "GoemonMenu.png");
			}

			if (slots[(int)Player.CharacterID.Ebisumaru] == 0) {
				lightLeft.Visible = false;
				ebisumaruButton.TextureNormal = (Texture2D)GD.Load(spritePath + "EbisumaruMenu.png");
			}

			if (slots[(int)Player.CharacterID.Sasuke] == 0) {
				lightRight.Visible = false;
				sasukeButton.TextureNormal = (Texture2D)GD.Load(spritePath + "SasukeMenu.png");
			}

			ArrangeCursors();

			checkingCursors = false;
		}
	}


	private void ArrangeCursors()
	{
		int count1 = 0;
		int count2 = 0;
		int count3 = 0;

		for (int i = 0; i < GameManager.instance.playerCount; i++) {
			Cursor temp = cursors[i];

			if (temp.slot == (int)Player.CharacterID.Ebisumaru) {
				MoveCursorPosition(temp, cursorPosition1, 0, ref count1);
			}
			else if (temp.slot == (int)Player.CharacterID.Goemon) {
				MoveCursorPosition(temp, cursorPosition2, 1, ref count2);
			}
			else if (temp.slot == (int)Player.CharacterID.Sasuke) {
				MoveCursorPosition(temp, cursorPosition3, 2, ref count3);
			}
		}
	}

	private void MoveCursorPosition(Cursor temp, Node2D cursorPosition, int i, ref int count)
	{
		if (slots[i] == 1) {
			temp.sprite.Position = cursorPosition.GlobalPosition;
		}
		else if (slots[i] == 2) {
			if (count == 0) {
				Node2D pos = cursorPosition.GetNode<Node2D>("Position1");
				temp.sprite.Position = pos.GlobalPosition;
				count++;
			}
			else {
				Node2D pos = cursorPosition.GetNode<Node2D>("Position2");
				temp.sprite.Position = pos.GlobalPosition;
			}
		}
		else if (slots[i] == 3) {
			if (count == 0) {
				Node2D pos = cursorPosition.GetNode<Node2D>("Position1");
				temp.sprite.Position = pos.GlobalPosition;
				count++;
			}
			else if (count == 1) {
				Node2D pos = cursorPosition.GetNode<Node2D>("Position2");
				temp.sprite.Position = pos.GlobalPosition;
				count++;
			}
			else {
				Node2D pos = cursorPosition.GetNode<Node2D>("Position3");
				temp.sprite.Position = pos.GlobalPosition;
			}
		}
	}

	// When a character is selected then move any cursors on 
	// the selected character to next available character 
	public void MoveOtherCursors(int playerNum)
	{
		for (int i = 0; i < GameManager.instance.playerCount; i++) {
			Cursor temp = cursors[i];
			if (i != playerNum && cursors[i].slot == cursors[playerNum].slot) {
				temp.slot = FindNextAvailableSlot(temp.slot);
			}
		}
	}

	public int FindNextAvailableSlot(int currentSlot)
	{
		while (buttons[currentSlot].ButtonPressed) {
			if (currentSlot < 2) {
				currentSlot += 1;
			}
			else if (currentSlot == 2) {
				currentSlot = 0;
			}
		}

		int next = currentSlot;
		return next;
	}
}
