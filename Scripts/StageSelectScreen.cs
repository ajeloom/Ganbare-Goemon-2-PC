using Godot;
using System;

public partial class StageSelectScreen : Control
{
	private GameManager gm;
	private Button impactButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>("/root/GameManager");
		impactButton = GetNode<Button>("CanvasLayer/Impact");
		
		if (gm.playerNum > 1) {
			impactButton.Visible = false;
		}
		else {
			impactButton.Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LevelButtonPressed() {
		gm.inMenu = false;
		gm.stageStart = true;
		gm.isBossStage = false;
		gm.isImpactStage = false;
		gm.GoToScene("res://Scenes/Stage1.tscn");
	}

	private void BossButtonPressed() {
		gm.inMenu = false;
		gm.stageStart = true;
		gm.isBossStage = true;
		gm.isImpactStage = false;
		gm.GoToScene("res://Scenes/boss.tscn");
	}

	private void ImpactButtonPressed() {
		gm.inMenu = false;
		gm.stageStart = true;
		gm.isBossStage = false;
		gm.isImpactStage = true;
		gm.GoToScene("res://Scenes/ImpactBattle.tscn");
	}
}
