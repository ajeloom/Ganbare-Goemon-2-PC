using Godot;
using System;

public partial class TitleScreen : Control
{
	public Node gameScene;
	[Export] private Control titleScreen;
	[Export] private Control characterScreen;
	[Export] private Control stageScreen;

	private Control previousScreen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleScreen.Visible = true;
		characterScreen.Visible = false;
		stageScreen.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayButtonPressed() {
		previousScreen = titleScreen;
		titleScreen.Visible = false;
		characterScreen.Visible = true;
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}

	private void CharacterButtonPressed() {
		characterScreen.Visible = false;
		stageScreen.Visible = true;
	}

	private void BackButtonPressed() {
		// Go to previous screen
		titleScreen.Visible = true;
		characterScreen.Visible = false;
	}

	private void LevelButtonPressed() {
		gameScene = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn").Instantiate();
		GetTree().Root.AddChild(gameScene);
		QueueFree();
	}

	private void BossButtonPressed() {
		gameScene = ResourceLoader.Load<PackedScene>("res://Scenes/boss.tscn").Instantiate();
		GetTree().Root.AddChild(gameScene);
		QueueFree();
	}

}

