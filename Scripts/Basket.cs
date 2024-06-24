using Godot;
using System;

public partial class Basket : Node2D
{
	private AnimationPlayer animPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!animPlayer.IsPlaying()) {
			QueueFree();
		}
	}
}
