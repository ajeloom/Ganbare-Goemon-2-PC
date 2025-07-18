using Godot;
using System;

public partial class PlayerNumberScreen : Control
{
	private int playerNum;

	private Button button1, button2, button3, nextButton;
	private GameManager gm;
	private AudioComponent audioComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button1 = GetNode<Button>("CanvasLayer/1");
		button2 = GetNode<Button>("CanvasLayer/2");
		button3 = GetNode<Button>("CanvasLayer/3");
		nextButton = GetNode<Button>("CanvasLayer/NextButton");
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");
		gm.inTitleScreen = false;
		gm.inMenu = true;
		gm.inStage = false;
		nextButton.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OneButtonPressed() {
		PlayButtonClickedSFX();
		playerNum = 1;
		button1.ButtonPressed = true;
		button2.ButtonPressed = false;
		button3.ButtonPressed = false;
		nextButton.Visible = true;
	}

	private void TwoButtonPressed() {
		PlayButtonClickedSFX();
		playerNum = 2;
		button1.ButtonPressed = false;
		button2.ButtonPressed = true;
		button3.ButtonPressed = false;
		nextButton.Visible = true;
	}

	private void ThreeButtonPressed() {
		PlayButtonClickedSFX();
		playerNum = 3;
		button1.ButtonPressed = false;
		button2.ButtonPressed = false;
		button3.ButtonPressed = true;
		nextButton.Visible = true;
	}

	private void NextButtonPressed() {
		PlayButtonClickedSFX();
		gm.SetNum(playerNum);
		gm.Transition("res://Scenes/CharacterSelectScreen.tscn");
	}

	private void BackButtonPressed() {
		PlayButtonClickedSFX();
		gm.Transition("res://Scenes/TitleScreen.tscn");
	}

	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX()
	{
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}
}
