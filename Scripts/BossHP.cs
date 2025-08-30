using Godot;
using System;

public partial class BossHP : CanvasLayer
{
	private HealthComponent healthComponent;
	private int health = 0;
	private bool checkHealth = false;
	private bool showHP = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Kabuki boss = GetParent<Kabuki>();
		healthComponent = boss.GetNode<HealthComponent>("HealthComponent");
		health = (int)healthComponent.health;

		for (int i = 0; i < health; i++) {
			PackedScene scene = GD.Load<PackedScene>("res://Scenes/BossHPBar.tscn");
			Node instance = scene.Instantiate();
			AddChild(instance);

			TextureProgressBar bar = GetChild<TextureProgressBar>(i);
			bar.Scale = new Vector2(10.0f, 10.0f);
			bar.Position = new Vector2(bar.Position.X + (3 * bar.Scale.X * i) + (1 * bar.Scale.X), bar.Position.Y + (1 * bar.Scale.Y));
			bar.MinValue = i;
			bar.MaxValue = i + 1;
			bar.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!showHP) {
			showHP = true;
			showHPAnimation();
		}
		
		if (!checkHealth) {
			checkHealth = true;
			for (int i = 0; i < health; i++) {
				TextureProgressBar bar = GetChild<TextureProgressBar>(i);
				bar.Value = healthComponent.health;
			}
			checkHealth = false;
		}

		if (healthComponent.health <= 0.0f) {
			QueueFree();
		}
	}

	private async void showHPAnimation() {
		for (int i = 0; i < health; i++) {
			TextureProgressBar bar = GetChild<TextureProgressBar>(i);
			bar.Visible = true;
			await ToSignal(GetTree().CreateTimer(0.025f), SceneTreeTimer.SignalName.Timeout);
		}
	}
}
