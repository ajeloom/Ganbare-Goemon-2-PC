using Godot;
using System;

public partial class AudioComponent : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void playSFX(string file, float volume) {
		Audio audio = new Audio(file, volume);
		AddChild(audio);
	}
}
