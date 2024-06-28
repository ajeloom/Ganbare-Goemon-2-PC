using Godot;
using System;

public partial class PlayerNumberScreen : Control
{
	private int playerNum;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HSlider slider = GetNode<HSlider>("CanvasLayer/HSlider");
		Label playerNumLabel = GetNode<Label>("CanvasLayer/Number");
		playerNumLabel.Text = Convert.ToString(slider.Value);
		playerNum = (int)slider.Value;
	}

	private void ContinueButtonPressed() {
		var gm = GetNode<GameManager>("/root/GameManager");
		gm.setNum(playerNum);
		gm.GoToScene("res://Scenes/CharacterSelectScreen.tscn");
	}
}
