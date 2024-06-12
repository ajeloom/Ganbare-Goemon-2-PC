using Godot;
using System;

public partial class TitleScreen : Control
{
	public Node gameScene;
	[Export] private Control titleScreen;
	[Export] private Control characterScreen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleScreen.Visible = true;
		characterScreen.Visible = false;
		gameScene = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn").Instantiate();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayButtonPressed() {
		titleScreen.Visible = false;
		characterScreen.Visible = true;
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}

	private void CharacterButtonPressed() {
		GetTree().Root.AddChild(gameScene);
		QueueFree();
	}

	private void BackButtonPressed() {
		titleScreen.Visible = true;
		characterScreen.Visible = false;
	}

}

