using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export] private Control pauseMenu;
	[Export] private AudioStreamPlayer audio;

	private bool isBossStage = true;

	private int playerNum = 3;

	private string[] stage = {
		"res://Scenes/Level1.tscn"
	};

	private string[] song = {
		"res://Sounds/Music/Boss.mp3"
	};

	private int level = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//pauseMenu.Visible = false;

		// // Load map
		// loadScene(stage[level]);

		// // Load player
		// for (int i = 0; i < playerNum; i++) {
		// 	loadScene("res://Scenes/Player.tscn");
		// }
		
		
		// // Set player location to spawn point
		// Player player = GetChild<Player>(4);
		// player.Position = new Vector2(93.0f, 247.0f);

		// Load music for stage
		// audio.Stream = (AudioStream)ResourceLoader.Load(song[level]);
		// audio.VolumeDb = -20.0f;
		// audio.Playing = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause")) {
			pauseMenu.Visible = true;
			GetTree().Paused = true;
		}
	}

	private void ResumeButtonPressed() {
		pauseMenu.Visible = false;
		GetTree().Paused = false;
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}

	private void loadScene(string name) {
		PackedScene var = GD.Load<PackedScene>(name);
		Node2D node = var.Instantiate<Node2D>();
		AddChild(node);
	}
}
