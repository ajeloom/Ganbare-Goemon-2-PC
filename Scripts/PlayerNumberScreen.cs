using Godot;
using System;

public partial class PlayerNumberScreen : Control
{
	private int playerNum;

	private Button button1, button2, button3;
	private GameManager gm;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<Button>("CanvasLayer/1");
		button2 = GetNode<Button>("CanvasLayer/2");
		button3 = GetNode<Button>("CanvasLayer/3");
		gm = GetNode<GameManager>("/root/GameManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		gm.inMenu = true;
	}

	private void OneButtonPressed() {
		playerNum = Convert.ToUInt16(button1.Text);
		button1.ButtonPressed = true;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;
	}

	private void TwoButtonPressed() {
		playerNum = Convert.ToUInt16(button2.Text);
		button1.ButtonPressed = false;
		button2.ButtonPressed = true;
		button3.ButtonPressed = false;
	}

	private void ThreeButtonPressed() {
		playerNum = Convert.ToUInt16(button3.Text);
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = true;
	}

	private void ContinueButtonPressed() {
		if (button1.ButtonPressed || button2.ButtonPressed || button3.ButtonPressed) {
			gm.setNum(playerNum);
			gm.Transition("res://Scenes/CharacterSelectScreen.tscn");
		}
	}

	private void BackButtonPressed() {
		gm.Transition("res://Scenes/TitleScreen.tscn");
	}
}
