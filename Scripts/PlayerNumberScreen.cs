using Godot;
using System;

public partial class PlayerNumberScreen : Control
{
	private int playerNum;

	private TextureButton button1, button2, button3;
	private GameManager gm;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<TextureButton>("CanvasLayer/1");
		button2 = GetNode<TextureButton>("CanvasLayer/2");
		button3 = GetNode<TextureButton>("CanvasLayer/3");
		gm = GetNode<GameManager>("/root/GameManager");
		gm.inTitleScreen = false;
		gm.inMenu = true;
		gm.inStage = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OneButtonPressed() {
		playerNum = 1;
		button1.ButtonPressed = true;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;
	}

	private void TwoButtonPressed() {
		playerNum = 2;
		button1.ButtonPressed = false;
		button2.ButtonPressed = true;
		button3.ButtonPressed = false;
	}

	private void ThreeButtonPressed() {
		playerNum = 3;
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = true;
	}

	private void ContinueButtonPressed() {
		if (button1.ButtonPressed || button2.ButtonPressed || button3.ButtonPressed) {
			gm.SetNum(playerNum);
			gm.Transition("res://Scenes/CharacterSelectScreen.tscn");
		}
	}

	private void BackButtonPressed() {
		gm.Transition("res://Scenes/TitleScreen.tscn");
	}
}
