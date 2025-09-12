using Godot;
using System;

public partial class BossHP : CanvasLayer
{
	private HealthComponent healthComponent;
	private int health = 0;
	private bool checkHealth = false;
	private bool showHP = false;

	private float offset = 28.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Kabuki boss = GetParent<Kabuki>();
		healthComponent = boss.GetNode<HealthComponent>("HealthComponent");
		health = (int)healthComponent.health;

		AddHealth();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (healthComponent.health <= 0.0f) {
			QueueFree();
		}
		else {
			if (!checkHealth) {
				checkHealth = true;
				for (int i = 1; i <= health; i++) {
					TextureProgressBar bar = GetChild<TextureProgressBar>(i);
					bar.Value = healthComponent.health;
				}
				checkHealth = false;
			}
		}
	}

	private void AddHealth()
	{
		for (int i = 1; i <= health; i++) {
			PackedScene scene = GD.Load<PackedScene>("res://Scenes/BossHPBar.tscn");
			Node instance = scene.Instantiate();
			AddChild(instance);

			TextureProgressBar bar = GetChild<TextureProgressBar>(i);
			bar.Position = new Vector2(bar.Position.X + (4 * bar.Scale.X * i) + (1 * bar.Scale.X) + offset, bar.Position.Y + (1 * bar.Scale.Y));
			bar.MinValue = i - 1;
			bar.MaxValue = i;
			bar.Visible = false;
		}

		showHPAnimation();
	}

	private async void showHPAnimation()
	{
		for (int i = 1; i <= health; i++) {
			TextureProgressBar bar = GetChild<TextureProgressBar>(i);
			bar.Visible = true;
			await ToSignal(GetTree().CreateTimer(0.025f), SceneTreeTimer.SignalName.Timeout);
		}
	}
}
