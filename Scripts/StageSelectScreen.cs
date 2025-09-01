using Godot;
using System;

public partial class StageSelectScreen : Control
{
	private TextureButton stageButton, bossButton, impactButton;
	private Button startButton;
	private AudioComponent audioComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		stageButton = GetNode<TextureButton>("CanvasLayer/Level 1");
		bossButton = GetNode<TextureButton>("CanvasLayer/Boss");
		impactButton = GetNode<TextureButton>("CanvasLayer/Impact");
		startButton = GetNode<Button>("CanvasLayer/StartButton");
		startButton.Visible = false;

		if (GameManager.instance.playerCount > 1) {
			impactButton.Disabled = true;
			impactButton.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LevelButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.SetStage((int)GameManager.StageNumber.Stage1);
		stageButton.ButtonPressed = true;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = false;
		ShowStartButton();
	}

	private void BossButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.SetStage((int)GameManager.StageNumber.BossStage);
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = true;
		impactButton.ButtonPressed = false;
		ShowStartButton();
	}

	private void ImpactButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.SetStage((int)GameManager.StageNumber.ImpactStage);
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = true;
		ShowStartButton();
	}

	private void StartButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.LoadStage();
	}

	private void BackButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.Transition("res://Scenes/CharacterSelectScreen.tscn");
	}

	private void ShowStartButton() {
		startButton.Visible = true;
	}
	
	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX() {
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}
}
