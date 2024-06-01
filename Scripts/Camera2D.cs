using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{
	[Export] CharacterBody2D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 vector = Position;
		vector.X = player.Position.X;

		Position = new Vector2(vector.X, 180.0f);	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 vector = Position;
		vector.X = player.Position.X;

		Position = new Vector2(vector.X, 180.0f);



		//velocity.X = direction.X * Speed;
	}
}
