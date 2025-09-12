using Godot;
using System;

public partial class Impact : Node2D
{
	private AnimationPlayer animPlayer;
	private HealthComponent healthComponent;
	private HitboxComponent left;
	private HitboxComponent right;
	private Sprite2D crosshair;
	private AudioComponent audio;


	private bool isPunching = false;
	private bool gotMousePosition = false;
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
		left = GetNode<HitboxComponent>("LeftHitboxComponent");
		right = GetNode<HitboxComponent>("RightHitboxComponent");

		bossHealth = GetNode<HealthComponent>("/root/Impact Battle/Senshuraku/HealthComponent");

		crosshair = GetNode<Sprite2D>("Crosshair");

		UpdateImpactHP(healthComponent.health);
		UpdateBossHP(bossHealth.health);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (healthComponent.health > 0.0f) {
			if (!gotMousePosition) {
				gotMousePosition = true;

				Vector2 mousePos = GetGlobalMousePosition();
				if (mousePos.Y < -55.5f) {
					mousePos.Y = -55.5f;
				}
				if (mousePos.Y > 25.5f) {
					mousePos.Y = 25.5f;
				}
				if (mousePos.X < -120.5f) {
					mousePos.X = -120.5f;
				}
				if (mousePos.X > 120.5f) {
					mousePos.X = 120.5f;
				}
				crosshair.GlobalPosition = new Vector2(mousePos.X, mousePos.Y);
				gotMousePosition = false;
			}
			
			if (Input.IsActionJustPressed("leftPunch") && !animPlayer.IsPlaying() && !isPunching) {
				isPunching = true;
				Vector2 mousePos = crosshair.GlobalPosition;

				// Limit the left arm to the left side
				if (mousePos.X > -15.0f) {
					mousePos.X = -15.0f;
				}
				if (mousePos.Y < -27.5f) {
					mousePos.Y = -27.5f;
				}

				left.GlobalPosition = new Vector2(mousePos.X - 52, mousePos.Y + 59);

				animPlayer.Play("LeftPunch");
				isPunching = false;
			}
			else if (Input.IsActionJustPressed("rightPunch") && !animPlayer.IsPlaying() && !isPunching) {
				isPunching = true;
				Vector2 mousePos = crosshair.GlobalPosition;

				// Limit the right arm to the right side
				if (mousePos.X < 15.0f) {
					mousePos.X = 15.0f;
				}
				if (mousePos.Y < -27.5f) {
					mousePos.Y = -27.5f;
				}

				// The +52 and +59 come from the hitbox position in the punch animation 
				right.GlobalPosition = new Vector2(mousePos.X + 52, mousePos.Y + 59);
				
				animPlayer.Play("RightPunch");
				isPunching = false;
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

	public void UpdateImpactHP(float hp)
	{
		Label label = GetNode<Label>("ImpactHealth");
		string health = hp.ToString();
		if (health.Length > 3) {
			label.Text = "999";
		}
		else if (health.Length == 3) {
			label.Text = health;
		}
		else if (health.Length == 2) {
			label.Text = "0" + health;
		}
		else {
			label.Text = "00" + health;
		}
	}

	public void UpdateBossHP(float hp)
	{
		Label label = GetNode<Label>("BossHealth");
		string health = hp.ToString();
		if (health.Length > 4) {
			label.Text = "9999";
		}
		else if (health.Length == 4) {
			label.Text = health;
		}
		else if (health.Length == 3) {
			label.Text = "0" + health;
		}
		else if (health.Length == 2) {
			label.Text = "00" + health;
		}
		else {
			label.Text = "000" + health;
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
