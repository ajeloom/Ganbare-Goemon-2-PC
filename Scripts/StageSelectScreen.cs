using Godot;
using System;

public partial class StageSelectScreen : Control
{
	private AudioComponent audioComponent;
	private TextureRect stageSprite, extendedBG;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		stageSprite = GetNode<TextureRect>("CanvasLayer/StageSprite");
		extendedBG = GetNode<TextureRect>("CanvasLayer/ExtendedBG");

		GameManager.instance.SetStage((int)GameManager.StageNumber.Stage1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void StartButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.LoadStage();
	}

	private void BackButtonPressed() {
		PlayButtonClickedSFX();
		GameManager.instance.Transition("res://Scenes/CharacterSelectScreen.tscn");
	}

	private void ButtonMouseEntered() {
		audioComponent.playSFX("res://Sounds/SFX/MenuSelect.wav", -15.0f);
	}

	private void PlayButtonClickedSFX() {
		audioComponent.playSFX("res://Sounds/SFX/MenuClick.wav", -10.0f);
	}

	private void LeftArrowButtonPressed()
	{
		PlayButtonClickedSFX();
		GameManager.instance.PreviousStage();
		ChangeStagePicture();
	}

	private void RightArrowButtonPressed()
	{
		PlayButtonClickedSFX();
		GameManager.instance.NextStage();
		ChangeStagePicture();
	}

	private void ChangeStagePicture()
	{
		if (GameManager.instance.selectedStage == (int)GameManager.StageNumber.Stage1) {
			stageSprite.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Stage1.png");
			extendedBG.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Stage1_Blur.png");
		}
		else if (GameManager.instance.selectedStage == (int)GameManager.StageNumber.BossStage) {
			stageSprite.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Boss_Stage.png");
			extendedBG.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Boss_Stage_Blur.png");
		}
		else if (GameManager.instance.selectedStage == (int)GameManager.StageNumber.ImpactStage) {
			stageSprite.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Impact_Stage.png");
			extendedBG.Texture = (Texture2D)GD.Load("res://Sprites/UI/Menu/StageSelect/Impact_Stage_Blur.png");
		}
	}
}
