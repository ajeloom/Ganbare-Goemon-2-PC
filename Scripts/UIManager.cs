using Godot;
using System;

public partial class UIManager : Node2D
{
	int time = 99;
	[Export] private Label timeLabel;
	[Export] private Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.Start();
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
			GetTree().Quit();
		}
		
	}

	private void OnTimerTimeout() {
		time--;
	}
}
