using Godot;
using System;

public partial class GameManager : Node2D
{
	private Control pauseMenu;
	public AudioStreamPlayer audio;
	private CanvasLayer canvas;
	private Control transition;
	private AnimationPlayer transitionAP;

	public Node currentScene { get; set; }

	public bool isPaused = false;
	public bool isImpactStage = false;
	public bool isBossStage = false;
	public bool stageStart = false;
	public bool selectCharacter = false;
	private bool initLoad = false;
	private bool checkPlayer = false;
	public bool addedCursors = false;
	public bool deletedCursors = false;
	private bool isTransitioning = false;
	public bool inMenu = false;
	private bool musicPlaying = false;

	public int playerNum { get; set; }
	public int characterNum { get; set; }

	public bool characterSelected = false;

	public player[] players;

	public struct player {
		public player(Player Node, int Character, int Slot, int Lives, int Coins, bool IsAlive) {
			node = Node;
			character = Character;
			slot = Slot;
			lives = Lives;
			coins = Coins;
			isAlive = IsAlive;
		}

		public Player node { get; set; }
		public int character { get; set; }
		public int slot { get; }
		public int lives { get; set; }
		public int coins { get; set; }
		public bool isAlive { get; set; }
	};

	private string[] stage = {
		"res://Scenes/Stage1.tscn"
	};

	private string[] song = {
		"res://Sounds/Music/Boss.mp3"
	};

	public bool endLevel = false;
	public bool canPause = true;
	private bool exitGame = false;
	public bool reloadingLevel = false;
	private bool gameOver = false;
	private bool loadedMusic = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Viewport root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);

		pauseMenu = GetNode<Control>("/root/GameManager/CanvasLayer/PauseMenu");
		pauseMenu.Visible = false;

		canvas = GetNode<CanvasLayer>("/root/GameManager/UI/CanvasLayer");
		canvas.Visible = false;

		audio = GetNode<AudioStreamPlayer>("BG Music");
		
		transition = GetNode<Control>("Fade Transition");
		transitionAP = transition.GetNode<AnimationPlayer>("CanvasLayer/AnimationPlayer");

		// Initialize player array
		players = new player[3];

		// Load title screen
		GoToScene("res://Scenes/TitleScreen.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Change the layer of the transition control node to not block any buttons
		if (isTransitioning) {
			CanvasLayer layer = transition.GetNode<CanvasLayer>("CanvasLayer");
			layer.Layer = 20;
		}
		else {
			CanvasLayer layer = transition.GetNode<CanvasLayer>("CanvasLayer");
			layer.Layer = 0;
		}

		// In-game
		if (stageStart) {
			if (isImpactStage) {
				if (!initLoad) {
					initLoad = true;
					// Load fade transition
					getTransition("FadeIn");
				}

				if (Input.IsActionJustPressed("pause") && canPause && !isPaused) {
					isPaused = true;
					Input.MouseMode = Input.MouseModeEnum.Visible;
					pauseMenu.Visible = true;
					GetTree().Paused = true;
				}
				else if (Input.IsActionJustPressed("pause") && isPaused) {
					ResumeButtonPressed();
				}

				// Load stage music
				if (!loadedMusic) {
					loadedMusic = true;
					audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/ImpactBoss.mp3");
					audio.VolumeDb = -10.0f;
					audio.Play();
				}

				if (endLevel) {
					closeGame();
				}
					
			}
			else {
				if (!initLoad) {
					initLoad = true;				
					
					// Load fade transition
					getTransition("FadeIn");

					// Show the bottom UI
					canvas.Visible = true;

					LoadPlayers(false);
				}

				// Keep track of player lives/coins
				for (int i = 0; i < playerNum; i++) {
					players[i].coins = players[i].node.coins;
					players[i].lives = players[i].node.lives;
					players[i].isAlive = players[i].node.isAlive;

					// Load game over
					if (!players[0].isAlive && !players[1].isAlive && !players[2].isAlive) {
						if (players[i].lives >= 0) {
							reloadScene();
						}
						else if (players[0].lives == -1 && players[1].lives == -1 && players[2].lives == -1) {
							if (!gameOver) {
								gameOver = true;
								GameOver();	
							}
						}
					}
				}

				if (Input.IsActionJustPressed("pause") && canPause && !isPaused) {
					isPaused = true;
					pauseMenu.Visible = true;
					GetTree().Paused = true;
				}
				else if (Input.IsActionJustPressed("pause") && isPaused) {
					ResumeButtonPressed();
				}

				if (endLevel) {
					closeGame();
				}
			}
		}
		else {
			if (inMenu) {
				if (!musicPlaying) {
					musicPlaying = true;
					audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/FileSelect.mp3");
					audio.VolumeDb = -5.0f;
					audio.Play();
				}
			}
			else {
				// Stop the music if you go back to the title screen
				if (!stageStart) {
					musicPlaying = false;
					audio.Stop();
				}	
			}

			// You enter the CSS
			if (selectCharacter) {
				pickCharacter();
			}
			else {
				deleteCursors();
			}
		}
	}

	private void ResumeButtonPressed() {
		isPaused = false;
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

	// Sets the character for each player
	public void setCharacter(int playerNum, int value) {
		players[playerNum].character = value;
	}

	private async void closeGame() {
		if (!exitGame) {
			exitGame = true;
			getTransition("FadeOut");

			await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);
			GetTree().Quit();
		}
	}

	private async void reloadScene() {
		if (!reloadingLevel) {
			reloadingLevel = true;
			stageStart = false;
			
			getTransition("FadeOut");

			audio.Stop();

			await ToSignal(GetTree().CreateTimer(2.8f), SceneTreeTimer.SignalName.Timeout);
			
			for (int i = 0; i < playerNum; i++) {
				Player temp = players[i].node;
				RemoveChild(temp);
			}

			if (isBossStage) {
				GoToScene("res://Scenes/boss.tscn");
				stageStart = true;
				isBossStage = true;
			}
			else {
				GoToScene("res://Scenes/Stage1.tscn");
				stageStart = true;
				isBossStage = false;
			}

			LoadPlayers(true);

			getTransition("FadeIn");
			reloadingLevel = false;
		}
	}

	public void getTransition(string name) {
		if (!isTransitioning) {
			isTransitioning = true;
			AnimationPlayer animPlayer = transition.GetNode<AnimationPlayer>("CanvasLayer/AnimationPlayer");
			animPlayer.Play(name);
		}
	}

	// For transitioning between two scenes
	public async void Transition(string nextScene) {
		if (!isTransitioning) {
			isTransitioning = true;
			transitionAP.Play("Fade");

			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

			GoToScene(nextScene);

			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			
			isTransitioning = false;
		}
	}

	private async void GameOver() {
		// Wait for all players to be off screen
		await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);
		getTransition("FadeOut");
		
		// Wait for fade out transition to end
		await ToSignal(GetTree().CreateTimer(1.4f), SceneTreeTimer.SignalName.Timeout);
		getTransition("FadeIn");
		AddChild("res://Scenes/GameOverScreen.tscn", this);
		
		await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
		closeGame();
	}

	private void LoadPlayers(bool isReloading) {
		for (int i = 0; i < 3; i++) {
			if (i < playerNum) {
				if (players[i].character == 0) {
					AddChild("res://Scenes/Goemon.tscn", this);
				}
				else if (players[i].character == 1) {
					AddChild("res://Scenes/Ebisumaru.tscn", this);
				}
				else if (players[i].character == 2) {
					AddChild("res://Scenes/Sasuke.tscn", this);
				}

				Player temp = GetChild<Player>(GetChildCount() - 1);
				temp.playerNum = i;
				
				if (isReloading) {
					temp.coins = players[i].coins;
					temp.lives = players[i].lives;
					temp.chara = players[i].character;
					players[i].node = temp;
					if (players[i].lives > -1) {
						temp.isAlive = true;
					}
					else {
						temp.isAlive = false;
						temp.healthComponent.health = 0.0f;
						temp.Position = new Vector2(0.0f, 5000.0f);
						continue;
					}
				}
				else {
					// Initialize the variables
					temp.coins = 100;
					temp.lives = 2;
					temp.isAlive = true;
					temp.chara = players[i].character;
					players[i] = new player(temp, temp.chara, i, temp.lives, temp.coins, temp.isAlive);
				}

				// Get spawn point
				Node2D node;
				if (isBossStage) {
					node = GetTree().Root.GetNode<Node2D>("Boss Fight/SpawnPoint" + i);
				}
				else {
					node = GetTree().Root.GetNode<Node2D>("Stage1/Level1/SpawnPoint" + i);
				}
				
				// Set player location to spawn point
				temp.Position = node.GlobalPosition;
			}
			else {
				players[i] = new player(null, 0, i, -1, 0, false);
			}
		}

		// Boss stage has its own camera and no timer
		if (!isBossStage) {			
			AddChild("res://Scenes/Boundary.tscn", this);
			AddChild("res://Scenes/Boundary.tscn", this);

			// Set the camera to player
			AddChild("res://Scenes/Camera.tscn", this);

			// Start timer
			Timer timer = GetNode<Timer>("UI/Timer");
			UIManager ui = GetNode<UIManager>("UI");
			ui.time = 99;
			timer.Start();

			// Load stage music
			audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/Level_1_Ryukyu_Resort.mp3");
			audio.VolumeDb = -10.0f;
			audio.Play();
		}
		else {
			// Load stage music
			audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/Boss.mp3");
			audio.VolumeDb = -20.0f;
			audio.Play();
		}
	}
	
	public bool IsPlayerRespawning() {
		for (int i = 0; i < playerNum; i++) {
			if (players[i].lives > -1 && !players[i].isAlive) {
				return true;
			}
		}
		return false;
	}

	private void pickCharacter() {
		if (!addedCursors) {
			addedCursors = true;

			for (int i = 0; i < playerNum; i++) {
				var scene = GD.Load<PackedScene>("res://Scenes/Cursor.tscn");
				var instance = scene.Instantiate();
				AddChild(instance, true);

				// Access the child
				Cursor node = (Cursor)GetChild(GetChildCount() - 1);
				node.playerNum = i;

				Sprite2D sprite = node.GetNode<Sprite2D>("CanvasLayer/Sprite2D");
				sprite.Frame = i;
			}

			deletedCursors = false;
		}
	}

	public void deleteCursors() {
		if (!deletedCursors) {
			deletedCursors = true;

			for (int i = 0; i < playerNum; i++) {
				// Access the child
				Node2D node = (Node2D)GetChild(GetChildCount() - 1);
				RemoveChild(node);
			}

			addedCursors = false;
		}
	}
}
