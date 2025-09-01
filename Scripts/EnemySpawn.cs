using Godot;
using System;

public partial class EnemySpawn : Node2D
{
	private bool enemySpawned = false;
	[Export] private PackedScene packedScene;

	private bool checkedEnemies = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.instance.endingLevel && !checkedEnemies) {
			checkedEnemies = true;
			// Ignore childs that are not enemies
			for (int i = 1; i < GetChildCount(); i++) {
				Enemy enemy = GetChild<Enemy>(i);
				enemy.Die();
			}
		}
	}

	private void OnScreenEntered() {
		if (!enemySpawned
				&& GetChildCount() <= 1
				&& GameManager.instance.stageFullyLoaded
				&& !GameManager.instance.endingLevel) {
			Node instance = packedScene.Instantiate();
			AddChild(instance, true);
		}
	}

	private void OnScreenExited() {
		if (GetChildCount() <= 1) {
			enemySpawned = false;
		}
	}
}
