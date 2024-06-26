using Godot;
using System;

public partial class CharacterSelectScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GoemonSelected() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.GoToScene("res://Scenes/StageSelectScreen.tscn");
	}

	private void BackButtonPressed() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.GoToScene("res://Scenes/PlayerNumberScreen.tscn");
	}
}
