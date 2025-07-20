using Godot;
using System;

public partial class StageSelectScreen : Control
{
	private GameManager gm;
	private TextureButton stageButton, bossButton, impactButton;
	private Button startButton;
	private AudioComponent audioComponent;
	private string path;
	private bool bossStage = false;
	private bool impactStage = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		stageButton = GetNode<TextureButton>("CanvasLayer/Level 1");
		bossButton = GetNode<TextureButton>("CanvasLayer/Boss");
		impactButton = GetNode<TextureButton>("CanvasLayer/Impact");
		startButton = GetNode<Button>("CanvasLayer/StartButton");
		startButton.Visible = false;

		if (gm.playerNum > 1) {
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
		bossStage = false;
		impactStage = false;
		path = "res://Scenes/Stage1.tscn";
		stageButton.ButtonPressed = true;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = false;
		ShowStartButton();
	}

	private void BossButtonPressed() {
		PlayButtonClickedSFX();
		bossStage = true;
		impactStage = false;
		path = "res://Scenes/boss.tscn";
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = true;
		impactButton.ButtonPressed = false;
		ShowStartButton();
	}

	private void ImpactButtonPressed() {
		PlayButtonClickedSFX();
		bossStage = false;
		impactStage = true;
		path = "res://Scenes/ImpactBattle.tscn";
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = true;
		ShowStartButton();
	}

	private async void StartButtonPressed() {
		PlayButtonClickedSFX();
		gm.isBossStage = bossStage;
		gm.isImpactStage = impactStage;
		gm.Transition(path);
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		gm.gameState = GameManager.State.Stage;
	}

	private void BackButtonPressed() {
		PlayButtonClickedSFX();
		gm.Transition("res://Scenes/CharacterSelectScreen.tscn");
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
