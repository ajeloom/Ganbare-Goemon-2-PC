using Godot;
using System;

public partial class Explosion : Node2D
{
	private AnimationPlayer animPlayer;
	private bool isPlayingAnim = false;

	public int explosion = 0;
	public bool biggestExplosion = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isPlayingAnim) {
			isPlayingAnim = true;
			if (explosion == 0) {
				animPlayer.Play("Explosion");	
			}
			else if (explosion == 1) {
				animPlayer.Play("BigExplosion");
			}
			else if (explosion == 2) {
				animPlayer.Play("LargeExplosion");
			}
		}
		
		if (!animPlayer.IsPlaying()) {
			QueueFree();
		}
	}
}
