using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export] private Control pauseMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pauseMenu.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause")) {
			pauseMenu.Visible = true;
			GetTree().Paused = true;
		}
	}

	private void ResumeButtonPressed() {
		pauseMenu.Visible = false;
		GetTree().Paused = false;
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}
}
