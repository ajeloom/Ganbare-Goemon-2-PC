using Godot;
using System;

public partial class Pass : Node2D
{
	private AudioStreamPlayer stageClearAudio;
	private AudioStreamPlayer collectAudio;
	private bool gotPass = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = true;
		stageClearAudio = GetNode<AudioStreamPlayer>("StageClearAudio");
		collectAudio = GetNode<AudioStreamPlayer>("CollectAudio");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void AreaEntered(Area2D area)
	{
		if (!gotPass) {
			gotPass = true;

			GameManager.instance.canPause = false;

			collectAudio.Play();

			Visible = false;

			GameManager.instance.audio.Stop();

			Timer timer = GameManager.instance.GetNode<Timer>("UI/Timer");
			timer.Stop();

			GameManager.instance.endingLevel = true;

			stageClearAudio.Play();
			await ToSignal(GetTree().CreateTimer(4.5f), SceneTreeTimer.SignalName.Timeout);
			GameManager.instance.GoToMenu();
		}
	}
}
