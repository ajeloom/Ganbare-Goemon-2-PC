using Godot;
using System;

public partial class TreeTop : AnimatedSprite2D
{
	private void OnScreenEntered()
	{
		Play();
	}

	private void OnScreenExited()
	{
		Stop();
	}
}
