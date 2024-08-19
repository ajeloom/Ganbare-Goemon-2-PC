using Godot;
using System;

public partial class StageSelectScreen : Control
{
	private GameManager gm;
	private TextureButton stageButton, bossButton, impactButton;
	private string path;
	private bool bossStage = false;
	private bool impactStage = false;
	private bool stageSelected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");
		stageButton = GetNode<TextureButton>("CanvasLayer/Level 1");
		bossButton = GetNode<TextureButton>("CanvasLayer/Boss");
		impactButton = GetNode<TextureButton>("CanvasLayer/Impact");
		
		if (gm.playerNum > 1) {
			impactButton.Disabled = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LevelButtonPressed() {
		bossStage = false;
		impactStage = false;
		path = "res://Scenes/Stage1.tscn";
		stageButton.ButtonPressed = true;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = false;

		stageSelected = true;
	}

	private void BossButtonPressed() {
		bossStage = true;
		impactStage = false;
		path = "res://Scenes/boss.tscn";
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = true;
		impactButton.ButtonPressed = false;

		stageSelected = true;
	}

	private void ImpactButtonPressed() {
		bossStage = false;
		impactStage = true;
		path = "res://Scenes/ImpactBattle.tscn";
		stageButton.ButtonPressed = false;
		bossButton.ButtonPressed = false;
		impactButton.ButtonPressed = true;

		stageSelected = true;
	}

	private void ContinueButtonPressed() {
		if (stageSelected) {
			gm.inMenu = false;
			gm.stageStart = true;
			gm.isBossStage = bossStage;
			gm.isImpactStage = impactStage;
			gm.GoToScene(path);
		}
	}

	private void BackButtonPressed() {
		gm.Transition("res://Scenes/CharacterSelectScreen.tscn");
	}
}
