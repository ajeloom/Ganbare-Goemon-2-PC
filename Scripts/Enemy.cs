using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float speed = 300.0f;
	[Export] public float jumpVelocity = -400.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		if (IsOnFloor())
			velocity.Y = jumpVelocity;

		Velocity = velocity;
		MoveAndSlide();
	}
}
