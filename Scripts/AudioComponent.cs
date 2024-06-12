using Godot;
using System;

public partial class AudioComponent : AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Playing) {
			QueueFree();
		}
	}

	public AudioComponent(string file, float volume) {
		Stream = (AudioStream)ResourceLoader.Load(file);
		VolumeDb = volume;
	}
}
