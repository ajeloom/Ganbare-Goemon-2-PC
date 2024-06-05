using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	public bool takingDamage = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnAreaEntered(Area2D area) {
		if (!takingDamage && area.IsInGroup("Hitbox")) {
			takingDamage = true;

			if (this.IsInGroup("Player"))
				await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
			else if (this.IsInGroup("Enemy"))
				await ToSignal(GetTree().CreateTimer(0.35f), SceneTreeTimer.SignalName.Timeout);
			takingDamage = false;
		}
	}
}
