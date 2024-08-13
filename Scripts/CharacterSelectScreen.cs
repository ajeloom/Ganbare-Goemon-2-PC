using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	private TextureButton button1, button2, button3;
	private int characterNum;
	private int playerNum;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<TextureButton>("CanvasLayer/Goemon");
		button2 = GetNode<TextureButton>("CanvasLayer/Ebisumaru");
		button3 = GetNode<TextureButton>("CanvasLayer/Sasuke");

		var gm = GetNode<GameManager>("/root/GameManager");
		playerNum = gm.playerNum;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (button1.ButtonPressed) {
			characterNum = 0;
		}
		else if (button2.ButtonPressed) {
			characterNum = 1;
		}
		else if (button3.ButtonPressed) {
			characterNum = 2;
		}
	}

	private void GoemonButtonPressed() {
		button1.ButtonPressed = true;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;
	}

	private void EbisumaruButtonPressed() {
		button1.ButtonPressed = false;
		button2.ButtonPressed = true;
		button3.ButtonPressed = false;
	}

	private void SasukeButtonPressed() {
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = true;
	}

	private void ContinueButtonPressed() {
		if (button1.ButtonPressed || button2.ButtonPressed || button3.ButtonPressed) {
			var gm = GetNode<GameManager>("/root/GameManager");
			gm.deleteCursors();
			gm.selectCharacter = false;
			gm.GoToScene("res://Scenes/StageSelectScreen.tscn");
		}
	}

	private void BackButtonPressed() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.selectCharacter = false;
		gm.GoToScene("res://Scenes/PlayerNumberScreen.tscn");
	}

	public int getCharaNum() {
		return characterNum;
	}
}
