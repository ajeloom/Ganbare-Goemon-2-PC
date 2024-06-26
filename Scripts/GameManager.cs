using Godot;
using System;

public partial class GameManager : Node2D
{
	private Control pauseMenu;
	[Export] private AudioStreamPlayer audio;
	private CanvasLayer canvas;

	public Node currentScene { get; set; }

	public bool isBossStage = false;
	public bool stageStart = false;
	private bool loadedPlayers = false;
	private bool checkPlayer = false;

	public int playerNum { get; set; }

	public player[] players;

	public struct player {
		public player(int Slot, int Lives, int Coins, Player Node) {
			slot = Slot;
			lives = Lives;
			coins = Coins;
			node = Node;
		}

		public int slot { get; }
		public int lives { get; set; }
		public int coins { get; set; }
		public Player node { get; }
	};

	private string[] stage = {
		"res://Scenes/Stage1.tscn"
	};

	private string[] song = {
		"res://Sounds/Music/Boss.mp3"
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Viewport root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);

		pauseMenu = GetNode<Control>("/root/GameManager/CanvasLayer/PauseMenu");
		pauseMenu.Visible = false;

		canvas = GetNode<CanvasLayer>("/root/GameManager/UI/CanvasLayer");
		canvas.Visible = false;
		
		// Load title screen
		GoToScene("res://Scenes/TitleScreen.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (stageStart) {
			if (!loadedPlayers) {
				loadedPlayers = true;
				players = new player[playerNum];
				canvas.Visible = true;
				for (int i = 0; i < playerNum; i++) {
					int num = i + 1;

					// Load a player scene
					AddChild("res://Scenes/Player.tscn", this);

					// Initialize the variables
					Player temp = GetChild<Player>(GetChildCount() - 1);
					temp.playerNum = num;
					temp.coins = 100;
					temp.lives = 2;
					players[i] = new player(num, temp.lives, temp.coins, temp);

					// Get spawn point
					Node2D node;
					if (isBossStage) {
						node = GetTree().Root.GetNode<Node2D>("Boss Fight/SpawnPoint" + num);
					}
					else {
						node = GetTree().Root.GetNode<Node2D>("Stage1/Level1/SpawnPoint" + num);
					}
					
					// Set player location to spawn point
					temp.Position = node.GlobalPosition;
				}

				// Set the camera to player
				if (!isBossStage) {
					Player play = GetNode<Player>("Player");
					AddChild("res://Scenes/Camera.tscn", play);
				}
			}

			// Keep track of player lives/coins
			for (int i = 0; i < playerNum; i++) {
				players[i].coins = players[i].node.coins;
				players[i].lives = players[i].node.lives;
			}

			if (Input.IsActionJustPressed("pause")) {
				pauseMenu.Visible = true;
				GetTree().Paused = true;
			}
		}
	}

	private void ResumeButtonPressed() {
		pauseMenu.Visible = false;
		GetTree().Paused = false;
	}

	private void QuitButtonPressed() {
		GetTree().Quit();
	}

	public void AddChild(string name, Node parent) {
		var scene = GD.Load<PackedScene>(name);
		var instance = scene.Instantiate();
		parent.AddChild(instance, true);
	}

	public void LoadScene(string name) {
		// It is now safe to remove the current scene.
		currentScene.Free();

		// Load a new scene.
		var nextScene = GD.Load<PackedScene>(name);

		// Instance the new scene.
		currentScene = nextScene.Instantiate();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(currentScene);

		// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
		GetTree().CurrentScene = currentScene;
	}

	public void GoToScene(string path)
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:

		CallDeferred(MethodName.LoadScene, path);
	}

	public void setNum(int value) {
		playerNum = value;
	}
}
