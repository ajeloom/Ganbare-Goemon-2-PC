using Godot;
using System;

public partial class Impact : Node2D
{
	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private Node2D left;
	private Node2D right;
	private Sprite2D crosshair;
	private AudioComponent audio;

	private Control boss;
	private Control player;
	private Label thousandsLabel;
	private Label hundredsLabel;
	private Label tensLabel;
	private Label onesLabel;

	private bool isLeftUsed = false;
	private bool gotMousePosition = false;
	private bool gotHP = false;
	private bool spawnedExplosion = false;
	private bool bigExplosion = false;
	private int explosionNum = 0;

	public bool isAlive = true;

	private HealthComponent bossHealth;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		left = GetNode<Node2D>("Left");
		right = GetNode<Node2D>("Right");

		boss = GetNode<Control>("Boss HP");
		player = GetNode<Control>("Impact HP");
		bossHealth = GetNode<HealthComponent>("/root/Impact Battle/Senshuraku/HealthComponent");

		crosshair = GetNode<Sprite2D>("Crosshair");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update the boss hp
		GetHP(bossHealth, boss);
		GetHP(healthComponent, player);

		if (healthComponent.health > 0.0f) {
			if (!gotMousePosition) {
				gotMousePosition = true;

				Vector2 mousePos = GetGlobalMousePosition();
				if (mousePos.Y < -90.0f) {
					mousePos.Y = -90.0f;
				}
				if (mousePos.Y > 40.0f) {
					mousePos.Y = 40.0f;
				}
				if (mousePos.X < -180.0f) {
					mousePos.X = -180.0f;
				}
				if (mousePos.X > 180.0f) {
					mousePos.X = 180.0f;
				}
				crosshair.GlobalPosition = new Vector2(mousePos.X, mousePos.Y);
				gotMousePosition = false;
			}
			
			if (Input.IsActionJustPressed("leftPunch") && !animPlayer.IsPlaying()) {
				if (!isLeftUsed) {
					isLeftUsed = true;
					Vector2 mousePos = crosshair.GlobalPosition;

					// Limit the left arm to the left side
					if (mousePos.X > -15.0f) {
						mousePos.X = -15.0f;
					}
					if (mousePos.Y < -45.0f) {
						mousePos.Y = -45.0f;
					}

					left.GlobalPosition = new Vector2(mousePos.X - 85, mousePos.Y + 100);

					animPlayer.Play("leftPunch_2");
					isLeftUsed = false;
				}
				
			}
			else if (Input.IsActionJustPressed("rightPunch") && !animPlayer.IsPlaying()) {
				if (!isLeftUsed) {
					isLeftUsed = true;
					Vector2 mousePos = crosshair.GlobalPosition;

					// Limit the right arm to the right side
					if (mousePos.X < 15.0f) {
						mousePos.X = 15.0f;
					}
					if (mousePos.Y < -45.0f) {
						mousePos.Y = -45.0f;
					}

					right.GlobalPosition = new Vector2(mousePos.X + 85, mousePos.Y + 100);
					
					animPlayer.Play("rightPunch_2");
					isLeftUsed = false;
				}
				
			}
		}
		else {
			isAlive = false;
			GameManager.instance.canPause = false;
			SpawnExplosion();
			if (bigExplosion) {
				if (!spawnedExplosion) {
					spawnedExplosion = true;
					GameManager.instance.GoToMenu();
					PackedScene scene = GD.Load<PackedScene>("res://Scenes/Explosion.tscn");
					Explosion instance = scene.Instantiate<Explosion>();
					instance.explosion = 1;
					AddSibling(instance, true);
					Visible = false;
				}
				
			}
		}
	}

	private void GetHP(HealthComponent healthComponent, Control control) {
		if (!gotHP) {
			gotHP = true;

			Label thousandsLabel = control.GetNode<Label>("ThousandsPlace");
			Label hundredsLabel = control.GetNode<Label>("HundredsPlace");
			Label tensLabel = control.GetNode<Label>("TensPlace");
			Label onesLabel = control.GetNode<Label>("OnesPlace");

			string health = Convert.ToString(healthComponent.health);
			for (int i = 1; i <= health.Length; i++) {
				switch (i) {
					case 1:
						onesLabel.Text = health[health.Length - i].ToString();
						break;
					case 2:
						tensLabel.Text = health[health.Length - i].ToString();
						break;
					case 3:
						hundredsLabel.Text = health[health.Length - i].ToString();
						break;
					case 4:
						thousandsLabel.Text = health[health.Length - i].ToString();
						break;
				}
			}

			if (healthComponent.health < 1000.0f) {
				thousandsLabel.Text = Convert.ToString("0");
			}
			if (healthComponent.health < 100.0f) {
				hundredsLabel.Text = Convert.ToString("0");
			}
			if (healthComponent.health < 10.0f) {
				tensLabel.Text = Convert.ToString("0");
			}
			if (healthComponent.health < 1.0f) {
				onesLabel.Text = Convert.ToString("0");
			}

			gotHP = false;
		}		
	}

	private async void SpawnExplosion() {
		if (!spawnedExplosion && !bigExplosion) {
			spawnedExplosion = true;
			GameManager.instance.audio.Stop();
			
			PackedScene scene = GD.Load<PackedScene>("res://Scenes/Explosion.tscn");
			Node2D instance = scene.Instantiate<Node2D>();
			instance.GlobalPosition = new Vector2(GetRandomPosition(), GetRandomPosition());
			AddSibling(instance, true);
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

			explosionNum++;
			if (explosionNum == 10) {
				bigExplosion = true;
			}
			
			spawnedExplosion = false;
		}
		
	}

	private float GetRandomPosition() {
		Random rnd = new Random();
		int num = rnd.Next(100);

		int negative = rnd.Next(2);
		if (negative == 0) {
			num = -num;
		}
		return num; 
	}
}
