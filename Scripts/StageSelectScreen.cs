using Godot;
using System;

public partial class StageSelectScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LevelButtonPressed() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.GoToScene("res://Scenes/Stage1.tscn");
		gm.stageStart = true;
		gm.isBossStage = false;
	}

	private void BossButtonPressed() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.GoToScene("res://Scenes/boss.tscn");
		gm.stageStart = true;
		gm.isBossStage = true;
	}
}
