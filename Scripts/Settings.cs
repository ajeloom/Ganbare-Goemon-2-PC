using Godot;
using System;

public partial class Settings : Control
{
	private GameManager gm;
	private AudioComponent audioComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		gm = GetNode<GameManager>("/root/GameManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void BackButtonPressed()
	{
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
