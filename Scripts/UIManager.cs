using Godot;
using System;

public partial class UIManager : Control
{
	private int setTime = 99;
	private int timeLeft;
	private Label timeLabel;
	private Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeLabel = GetNode<Label>("CanvasLayer/Time Label");
		timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnTimerTimeout() {
		if (timeLeft > 0) {
			timeLeft--;

			DisplayTimeLeft();

			if (timeLeft == 0) {
				// Kill all players on screen
				for (int i = 0; i < GameManager.instance.playerCount; i++)
				{
					GameManager.instance.players[i].node.Die();
				}
			}
		}
	}

	private void DisplayTimeLeft()
	{
		if (timeLeft < 10) {
			timeLabel.Text = Convert.ToString("0" + timeLeft);
		}
		else {
			timeLabel.Text = Convert.ToString(timeLeft);
		}
	}

	public void SetTime(int time)
	{
		setTime = time;
	}

	public void ResetTimer()
	{
		timeLeft = setTime;
		DisplayTimeLeft();
	}
}
