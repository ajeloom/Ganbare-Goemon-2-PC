using Godot;
using System;

public partial class EnemySpawn : Node2D
{
	private bool enemySpawned = false;
	[Export] private PackedScene packedScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnScreenEntered() {
		if (!enemySpawned && GetChildCount() <= 1) {
			var instance = packedScene.Instantiate();
			AddChild(instance, true);
		}
	}

	private void OnScreenExited() {
		if (GetChildCount() <= 1) {
			enemySpawned = false;
		}
	}
}
