using Godot;
using System;

public partial class Cursor : Node2D
{
	public int playerNum { get; set; }
	public Sprite2D sprite;
	private AudioComponent audio;
	private CharacterSelectScreen css;

	[Export] public int slot = 0;

	public bool characterSelected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("CanvasLayer/Sprite2D");
		audio = GetNode<AudioComponent>("AudioComponent");
		css = GetNode<CharacterSelectScreen>("/root/Character Select Screen");
	}

	// Show mouse when it is moved
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion) {
			if (eventMouseMotion.Velocity != Vector2.Zero && eventMouseMotion.Position != Vector2.Zero) {
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (characterSelected) {
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				if (playerNum == (int)GameManager.PlayerNumber.Player1) {
					HideMouseCursor();
				}

				DeselectedCharacter();
			}
		}
		else {
			// Move the cursor
			if (Input.IsActionJustPressed("walkLeft" + playerNum.ToString())) {
				if (playerNum == (int)GameManager.PlayerNumber.Player1) {
					HideMouseCursor();
				}

				slot = ChangeSlot(slot, "Left");

				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}
			else if (Input.IsActionJustPressed("walkRight" + playerNum.ToString())) {
				if (playerNum == (int)GameManager.PlayerNumber.Player1) {
					HideMouseCursor();
				}

				slot = ChangeSlot(slot, "Right");

				audio.playSFX("res://Sounds/SFX/MenuSelect.wav", -18.0f);
			}

			// Player selects a character
			if (Input.IsActionJustPressed("jump" + playerNum.ToString())) {
				HideMouseCursor();

				if (!css.buttons[slot].ButtonPressed) {
					if (slot == (int)Player.CharacterID.Goemon) {
						SelectedCharacter("Goemon");
					}
					else if (slot == (int)Player.CharacterID.Ebisumaru) {
						SelectedCharacter("Ebisumaru");
					}
					else if (slot == (int)Player.CharacterID.Sasuke) {
						SelectedCharacter("Sasuke");
					}
				}
			}

			// Exit the CSS
			if (Input.IsActionJustPressed("attack" + playerNum.ToString())) {
				Input.MouseMode = Input.MouseModeEnum.Visible;
				css.BackButtonPressed();
			}
		}
	}

	public void SelectedCharacter(string characterName) {
		if (playerNum == (int)GameManager.PlayerNumber.Player1) {
			css.DisableButtons();
			css.ShowCancelButton(true);
		}

		audio.playSFX("res://Sounds/SFX/menuClick.wav", -5.0f);
		audio.playSFX("res://Sounds/SFX/" + characterName + "/selected.wav", -15.0f);
		GameManager.instance.SetCharacter(playerNum, slot);

		// Button is pressed
		css.buttons[slot].ButtonPressed = true;

		// Disable the button from being pressed with mouse
		css.buttons[slot].Disabled = true;

		// Move other cursors on character to different slot
		css.MoveOtherCursors(playerNum);

		characterSelected = true;
	}

	public void DeselectedCharacter() {
		if (playerNum == (int)GameManager.PlayerNumber.Player1) {
			// Enable the buttons to be pressed with mouse
			css.EnableButtons();
			css.ShowCancelButton(false);
		}
		else {
			css.buttons[slot].ButtonPressed = false;

			css.buttons[slot].Disabled = false;

			characterSelected = false;
		}
	}

	private void HideMouseCursor()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		Input.WarpMouse(new Vector2(0.0f, 0.0f));
	}

	private int ChangeSlot(int currentSlot, string direction)
	{
		if (direction.Capitalize() == "Right") {
			while (css.buttons[GetNextSlot(currentSlot)].ButtonPressed) {
				currentSlot = GetNextSlot(currentSlot);
			}
			
			currentSlot = GetNextSlot(currentSlot);
		}
		else if (direction.Capitalize() == "Left") {
			while (css.buttons[GetPreviousSlot(currentSlot)].ButtonPressed) {
				currentSlot = GetPreviousSlot(currentSlot);
			}
			
			currentSlot = GetPreviousSlot(currentSlot);
		}

		return currentSlot;
	}

	private int GetNextSlot(int slot)
	{
		if (slot < 2) {
			slot += 1;
		}
		else if (slot == 2) {
			slot = 0;
		}

		return slot;
	}
	
	private int GetPreviousSlot(int slot)
	{
		if (slot > 0) {
			slot -= 1;
		}
		else if (slot == 0) {
			slot = 2;
		}

		return slot;
	}
}
