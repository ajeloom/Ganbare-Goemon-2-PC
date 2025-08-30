using Godot;
using System;

public partial class Settings : Control
{
	string savePath = "user://settings.ini";
	private ConfigFile configFile = new ConfigFile();

	private AudioComponent audioComponent;
	[Export] private CheckBox CRTCheckBox;

	private bool displayCRTFilter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioComponent = GetNode<AudioComponent>("AudioComponent");

		// Load saved settings
		Load();

		CRTCheckBox.ButtonPressed = displayCRTFilter;
		CRTEffectCheckBoxToggled(CRTCheckBox.ButtonPressed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void Save()
	{
		configFile.Save(savePath);
	}

	private void Load()
	{
		// Load the saved settings
		Error error = configFile.Load(savePath);
		if (error != Error.Ok) {
			return;
		}

		displayCRTFilter = (bool)configFile.GetValue("Settings", "DisplayCRTFilter");
	}

	private void BackButtonPressed()
	{
		PlayButtonClickedSFX();
		GameManager.instance.Transition("res://Scenes/TitleScreen.tscn");
	}

	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX()
	{
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}

	private void CRTEffectCheckBoxToggled(bool toggledOn)
	{
		if (toggledOn && !GameManager.instance.HasNode("CRTFilter"))
		{
			PackedScene scene = GD.Load<PackedScene>("res://Scenes/CRTFilter.tscn");
			Node instance = scene.Instantiate();
			GameManager.instance.AddChild(instance);
		}
		else if (!toggledOn && GameManager.instance.HasNode("CRTFilter"))
		{
			CanvasLayer layer = GameManager.instance.GetNode<CanvasLayer>("CRTFilter");
			GameManager.instance.RemoveChild(layer);
		}
		
		configFile.SetValue("Settings", "DisplayCRTFilter", CRTCheckBox.ButtonPressed);
		Save();
	}
}
