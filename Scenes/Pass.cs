using Godot;
using System;

public partial class Pass : Node2D
{
	private AudioStreamPlayer audio;
	private bool gotPass = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = true;
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void AreaEntered(Area2D area) {
		if (!gotPass) {
			gotPass = true;

			Visible = false;

			var gm = GetNode<GameManager>("/root/GameManager");
			gm.audio.Stop();

			Timer timer = gm.GetNode<Timer>("UI/Timer");
			timer.Stop();

			audio.Play();
			await ToSignal(GetTree().CreateTimer(4.5f), SceneTreeTimer.SignalName.Timeout);
			gm.endLevel = true;
		}
		
	}
}
