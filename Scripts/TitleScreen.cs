using Godot;
using System;

public partial class TitleScreen : Control
{
	private GameManager gm;
	private AudioStreamPlayer audio;
	private AudioComponent audioComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");
		gm.inTitleScreen = true;
		gm.inMenu = false;
		gm.inStage = false;

		// Unpause the game if you return to title from pause screen
		GetTree().Paused = false;

		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayButtonPressed() {
		audio.Playing = false;
		PlayButtonClickedSFX();
		gm.Transition("res://Scenes/PlayerNumberScreen.tscn");
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}

	private void SettingsButtonPressed() {
		PlayButtonClickedSFX();
		gm.Transition("res://Scenes/Settings.tscn");
	}

	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX() {
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}
}