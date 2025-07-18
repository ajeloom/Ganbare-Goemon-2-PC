using Godot;
using System;

public partial class GameManager : Node2D
{
	private Control pauseMenu;
	public AudioStreamPlayer audio;
	private CanvasLayer canvas;
	private Control transition;
	private AnimationPlayer transitionAP;
	private AudioComponent audioComponent;

	public Node currentScene { get; set; }

	// Tells when the game is paused or not
	public bool isPaused = false; 

	// For controlling when a player can pause
	public bool canPause = true;
	
	// For loading players and UI at the start of the stage
	private bool initLoad = false;

	// Tells what part of the game you are in
	public bool inTitleScreen = false;
	public bool inMenu = false;
	public bool inStage = false;

	// For character select screen
	public bool selectCharacter = false;
	public bool addedCursors = false;
	public bool deletedCursors = false;

	// Tells when the game is transitioning between scenes
	private bool isTransitioning = false;

	// For changing the music in the AudioStreamPlayer
	private bool musicPlaying = false;
	
	// Tells what stage is being played
	public bool isImpactStage = false;
	public bool isBossStage = false;

	// For the end of the level
	public bool reloadingLevel = false;
	public bool endLevel = false;

	// For returning back to the title screen	
	private bool exitGame = false;
	
	// For when the game is currently in the game over screen
	private bool gameOver = false;

	// For loading the correct music for the stage
	private bool loadedMusic = false;

	public int playerNum { get; set; }
	public int characterNum { get; set; }

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
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		
		transition = GetNode<Control>("Fade Transition");
		transitionAP = transition.GetNode<AnimationPlayer>("CanvasLayer/AnimationPlayer");

		Input.MouseMode = Input.MouseModeEnum.Visible;

		// Initialize player array
		players = new player[3];

		// Load title screen
		GoToScene("res://Scenes/TitleScreen.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (inTitleScreen) {
			// Stop the time from going down
			Timer timer = GetNode<Timer>("UI/Timer");
			timer.Stop();

			// Stop the music if you go back to the title screen
			if (musicPlaying) {
				musicPlaying = false;
				audio.Stop();
			}
		}
		else if (inMenu) {
			if (!musicPlaying) {
				musicPlaying = true;
				audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/FileSelect.mp3");
				audio.VolumeDb = -5.0f;
				audio.Play();
			}

			// You enter the CSS
			if (selectCharacter) {
				PickCharacter();
			}
		}
		else if (inStage) {
			if (isImpactStage) {
				if (!initLoad) {
					initLoad = true;

					Input.MouseMode = Input.MouseModeEnum.Hidden;

					// Show the bottom UI
					canvas.Visible = false;
				}

				if (Input.IsActionJustPressed("pause") && canPause && !isPaused) {
					PauseGame();
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
					GoToMenu();
				}
			}
			else {
				if (!initLoad) {
					initLoad = true;

					// Show the bottom UI
					canvas.Visible = true;

					Input.MouseMode = Input.MouseModeEnum.Hidden;

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
								canPause = false;
								GameOver();	
							}
						}
					}
				}

				if (Input.IsActionJustPressed("pause") && canPause && !isPaused) {
					PauseGame();
				}
				else if (Input.IsActionJustPressed("pause") && isPaused) {
					ResumeButtonPressed();
				}

				if (endLevel) {
					GoToMenu();
				}
			}
		}

		// Change the layer of the transition control node to not block any buttons
		if (isTransitioning) {
			CanvasLayer layer = transition.GetNode<CanvasLayer>("CanvasLayer");
			layer.Layer = 20;
		}
		else {
			CanvasLayer layer = transition.GetNode<CanvasLayer>("CanvasLayer");
			layer.Layer = 10;
		}
	}

	private void PauseGame() {
		isPaused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		pauseMenu.Visible = true;
		GetTree().Paused = true;
	}

	private void ResumeButtonPressed() {
		PlayButtonClickedSFX();
		isPaused = false;
		pauseMenu.Visible = false;
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	private void MenuButtonPressed() {
		PlayButtonClickedSFX();
		pauseMenu.Visible = false;
		GoToMenu();
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

	public void SetNum(int value) {
		playerNum = value;
	}

	// Sets the character for each player
	public void SetCharacter(int playerNum, int value) {
		players[playerNum].character = value;
	}

	private async void GoToMenu() {
		if (!exitGame) {
			exitGame = true;

			Transition("res://Scenes/TitleScreen.tscn");

			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			
			Input.MouseMode = Input.MouseModeEnum.Visible;

			if (!isImpactStage) {
				// Remove extra nodes when in level 1
				if (!isBossStage) {
					for (int i = 0; i < 3; i++) {
						// Remove camera and boundaries
						Node2D temp = GetChild<Node2D>(GetChildCount() - 1);
						RemoveChild(temp);
					}
				}

				// Remove players
				for (int i = 0; i < playerNum; i++) {
					Node2D temp = GetChild<Node2D>(GetChildCount() - 1);
					RemoveChild(temp);
				}
			}
			
			// Reset values
			players = new player[3];

			isPaused = false;
			selectCharacter = false;
			initLoad = false;

			addedCursors = false;
			deletedCursors = false;

			endLevel = false;
			canPause = true;
			reloadingLevel = false;
			gameOver = false;
			loadedMusic = false;

			exitGame = false;
		}
	}

	private async void reloadScene() {
		if (!reloadingLevel) {
			reloadingLevel = true;
			canPause = false;
			inStage = false;			
			
			SmallTransition("FadeOut");

			audio.Stop();

			await ToSignal(GetTree().CreateTimer(2.8f), SceneTreeTimer.SignalName.Timeout);
			
			for (int i = 0; i < playerNum; i++) {
				Player temp = players[i].node;
				RemoveChild(temp);
			}

			if (isBossStage) {
				GoToScene("res://Scenes/boss.tscn");
				inStage = true;
				isBossStage = true;
			}
			else {
				GoToScene("res://Scenes/Stage1.tscn");
				inStage = true;
				isBossStage = false;
			}

			LoadPlayers(true);

			SmallTransition("FadeIn");
			canPause = true;
			reloadingLevel = false;
		}
	}

	// For transitioning during game reloads
	public async void SmallTransition(string name) {
		if (!isTransitioning) {
			isTransitioning = true;
			transitionAP.Play(name);

			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

			isTransitioning = false;
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
		SmallTransition("FadeOut");
		
		// Wait for fade out transition to end
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		GoToScene("res://Scenes/GameOverScreen.tscn");
		SmallTransition("FadeIn");

		await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
		GoToMenu();
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
					node = GetNode<Node2D>("/root/Boss Fight/SpawnPoint" + i);
				}
				else {
					node = GetNode<Node2D>("/root/Stage1/Level1/SpawnPoint" + i);
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

			// Reset time
			UIManager ui = GetNode<UIManager>("UI");
			ui.time = 99;
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

	private void PickCharacter() {
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

	public void DeleteCursors() {
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
	
	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX() {
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}
}
