using Godot;
using System;

public partial class UIManager : Node2D
{
	public int time = 99;
	private Label timeLabel;
	private Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeLabel = GetNode<Label>("CanvasLayer/Control/Time Label");
		timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (time < 10) {
			timeLabel.Text = Convert.ToString("0" + time);
		}
		else {
			timeLabel.Text = Convert.ToString(time);
		}

		if (time == 0) {
			timer.Stop();

			// Kill all players on screen
			for (int i = 0; i < GameManager.instance.playerCount; i++) {
				GameManager.instance.players[i].node.Die();
			}
		}
		
	}

	private void OnTimerTimeout() {
		if (time > 0)
			time--;
	}
}
