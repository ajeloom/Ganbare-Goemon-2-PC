using Godot;
using System;

public partial class TitleScreen : Control
{
	private GameManager gm;
	private AudioStreamPlayer audio;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
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
		gm.Transition("res://Scenes/PlayerNumberScreen.tscn");
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}
}