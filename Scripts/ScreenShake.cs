using Godot;
using System;

public partial class ScreenShake : Camera2D
{
	private float randomStrength = 5.0f;
	private float shakeFade = 5.0f;
	private float shakeStrength = 0.0f;

	RandomNumberGenerator rand = new RandomNumberGenerator();

	[Export] private Kabuki boss;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rand.Randomize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (boss.landed) {
			ApplyShake();
		}

		shakeStrength = Mathf.Lerp(shakeStrength, 0.0f, shakeFade * (float)delta);
		Offset = GetRandomOffset();
	}

	private void ApplyShake() {
		shakeStrength = randomStrength;
	}

	private Vector2 GetRandomOffset() {
		return new Vector2(rand.RandfRange(-shakeStrength, shakeStrength), 
		rand.RandfRange(-shakeStrength, shakeStrength));
	}
}
