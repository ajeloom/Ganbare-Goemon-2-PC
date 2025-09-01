using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float speed = 300.0f;
	[Export] public float jumpVelocity = -400.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	protected AnimationPlayer animPlayer;
	protected HealthComponent healthComponent;
	protected HitboxComponent hitboxComponent;
	protected AudioComponent audio;
	protected Sprite2D sprite;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		audio = GetNode<AudioComponent>("AudioComponent");
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

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

	public void Die()
	{
		healthComponent.health = 0.0f;
	}
}
