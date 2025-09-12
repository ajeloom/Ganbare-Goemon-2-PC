using Godot;
using System;

public partial class Kabuki : CharacterBody2D
{
	[Export] public float Speed = 100.0f;
	[Export] public float JumpVelocity = -200.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// For start up animation
	private bool waitedAtStart = false;
	private bool startUpAnimation = true;
	private Vector2 targetPosition;
	private bool emerged = false;

	private Vector2 direction;

	// For shaking the camera
	public bool landed = false;
	private bool playLandedAnimation = false;
	public bool shakeCamera = false;

	// For phase 1/2 attacks
	private bool isFlying = false;
	private bool startFlyingAttack = false;
	private bool spawnFlowers = false;
	private bool move = false;
	private bool moveLeft = true;
	private bool moveRight = false;
	private bool moveUp = true;
	private bool moveDown = false;

	private Vector2 startingPosition;
	private float startingHeight = -50.5f;
	private float flyingAttackHeight = -19.5f;

	// Components/Nodes
	private AnimationPlayer animPlayer;
	//[Export] private AnimationPlayer effectsPlayer;
	private HealthComponent healthComponent;
	private HurtboxComponent hurtboxComponent;
	private AudioComponent audioComponent;
	private RayCast2D rayCast;
	private Sprite2D bodySprite;
	private ScreenShake camera;
	private RayCast2D horizontalRayCast;

	private int phase = 1;

	private bool createNewHP = false;

	private bool takingDamage = false;

	private bool isLevelEnding = false;

	public override void _Ready()
	{
		PackedScene scene = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
		Node instance = scene.Instantiate();
		AddChild(instance);

		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
		hurtboxComponent.Scale = Vector2.Zero;
		audioComponent = GetNode<AudioComponent>("AudioComponent");
		rayCast = GetNode<RayCast2D>("RayCast2D");
		horizontalRayCast = GetNode<RayCast2D>("HorizontalRayCast2D");
		bodySprite = GetNode<Sprite2D>("Body");
		camera = GetNode<ScreenShake>("/root/Boss Fight/Camera2D");
		
		GlobalPosition = new Vector2(0.0f, startingHeight);
		targetPosition = new Vector2(0.0f, flyingAttackHeight);
		direction = Vector2.Left;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Wait a second before moving
		if (!waitedAtStart) {
			StartUpDelay(2.0f);
		}
		else {
			Vector2 velocity = Velocity;

			if (healthComponent.health > 0.0f) {
				if (phase == 1) {
					if (startUpAnimation) {
						if (!IsOnFloor() && GlobalPosition.Y <= flyingAttackHeight) {
							float currentGravity = 30.0f;
							velocity.Y += currentGravity * (float)delta;
						}
						else if (!IsOnFloor() && GlobalPosition.Y > flyingAttackHeight) {
							float currentGravity = 245.0f;
							velocity.Y += currentGravity * (float)delta;

							// Do startup animation
							hurtboxComponent.Scale = new Vector2(1.0f, 1.0f);
							if (!emerged) {
								emerged = true;
								animPlayer.Play("emerge");
							}
						}
						
						if (IsOnFloor()) {
							startUpAnimation = false;
						}
					}
					else {
						// Fall when in the air and not flying
						if (!IsOnFloor() && !isFlying) {
							float currentGravity = 245.0f;
							velocity.Y += currentGravity * (float)delta;
							landed = false;
						}

						if (IsOnFloor()) {
							// Shake the camera when landing
							if (!landed) {
								landed = true;
								camera.ApplyShake(2.0f);
							}

							// Do flower attack
							DoFlowerAttack();
						}

						// Fly in the air to target position
						if (animPlayer.CurrentAnimation == "hide") {
							targetPosition = new Vector2(Position.X, flyingAttackHeight);
							Vector2 dir = GlobalPosition.DirectionTo(targetPosition);
							
							if (Position >= targetPosition) {
								velocity = dir * 100.0f;
								isFlying = true;
							}
						}

						// Reset to target position for flying attack
						if (isFlying && Position <= targetPosition) {
							velocity = Vector2.Zero;
							animPlayer.Play("RESET");
							startFlyingAttack = true;
						}

						// Do flying attack
						if (startFlyingAttack) {
							playLandedAnimation = false;
							DoFlyingAttack();

							if (move) {
								velocity.X = direction.X * Speed;
							}
							else {
								velocity.X = 0.0f;
							}
						}
					}
				}
				else if (phase == 2) {
					// Fall when in the air
					if (!IsOnFloor()) {
						if (!startUpAnimation) {
							startUpAnimation = true;
							animPlayer.Play("land");
						}
						velocity.Y += gravity * 0.5f * (float)delta;
						landed = false;
					}

					// Switch to jumping attack
					if (IsOnFloor()) {
						if (!landed) {
							landed = true;
							camera.ApplyShake(2.0f);
						}
						velocity.Y = JumpVelocity;
						DoJumpingAttack();

						// Move left and right
						if (move) {
							velocity.X = direction.X * Speed;
						}
						else {
							velocity.X = 0.0f;
						}

						audioComponent.playSFX("res://Sounds/SFX/Goemon/jump.wav", -25.0f);
					}

					if (takingDamage) {
						animPlayer.Play("hurt");
					}
					else {
						animPlayer.Play("jump");
					}
				}
			}
			else {
				if (phase == 1) {
					if (!createNewHP) {
						createNewHP = true;
						BossHP bossHP = GetNode<BossHP>("Boss HP");
						bossHP.QueueFree();
						healthComponent.health = 32.0f;
						PackedScene temp = GD.Load<PackedScene>("res://Scenes/BossHP.tscn");
						Node instance = temp.Instantiate();
						AddChild(instance);
					}
					
					// Spawn a basket with explosion
					PackedScene scene = GD.Load<PackedScene>("res://Scenes/Basket.tscn");
					Node2D node = scene.Instantiate<Node2D>();
					node.Position = Position;
					AddSibling(node);

					isFlying = false;
					startUpAnimation = false;
					velocity.X = 0.0f;

					TurnTowardsPlayer();

					phase = 2;
				}
				else if (phase == 2) {
					if (!IsOnFloor()) {
						velocity.Y += gravity * (float)delta;
						landed = false;
					}

					velocity.X = 0.0f;
					EndLevel();
				}
			}

			takingDamage = hurtboxComponent.tookDamage;

			Velocity = velocity;
			MoveAndSlide();
		}	
	}

	private async void StartUpDelay(float time)
	{
		// Wait one second
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
		waitedAtStart = true;
	}
	

	private async void DoFlowerAttack()
	{
		// Gravity is normal
		// startUpAnimation = false;

		if (!playLandedAnimation) {
			playLandedAnimation = true;
			animPlayer.Play("land");
			animPlayer.Queue("open");
			animPlayer.Queue("close");
			animPlayer.Queue("hide");	
		}

		// Spawn flowers
		if (animPlayer.CurrentAnimation == "open") {	
			if (!spawnFlowers) {
				spawnFlowers = true;
				await ToSignal(GetTree().CreateTimer(0.9f), SceneTreeTimer.SignalName.Timeout);
				int temp = 1;

				// Shoot a group of 8 flowers, 3 times
				for (int j = 0; j < 3; j++) {
					for (int i = 0; i < 8; i++) {
						// Position them in evenly above the boss
						PackedScene scene = GD.Load<PackedScene>("res://Scenes/Flower.tscn");
						Node2D node = scene.Instantiate<Node2D>();
						if (i < 4) {
							node.Position = new Vector2(Position.X - (6 * temp), Position.Y - 57.5f);
							temp += 1;
						}
						else {
							node.Position = new Vector2(Position.X + (6 * temp), Position.Y - 57.5f);
							temp += 1;
						}

						if (temp == 5) {
							temp = 1;
						}
						AddSibling(node);
					}
					await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				}
			}			
		}
		else if (animPlayer.CurrentAnimation == "close") {
			spawnFlowers = false;
		}
	}

	private void TurnTowardsPlayer()
	{
		Node2D player = GameManager.instance.players[(int)GameManager.PlayerNumber.Player1].node;
		if (player.GlobalPosition > GlobalPosition)
		{
			direction = Vector2.Right;
			horizontalRayCast.TargetPosition = new Vector2(32.0f, 0.0f);
			bodySprite.FlipH = true;
		}
		else
		{
			direction = Vector2.Left;
			horizontalRayCast.TargetPosition = new Vector2(-32.0f, 0.0f);
			bodySprite.FlipH = false;
		}
	}

	private void DoFlyingAttack()
	{
		// Go left first
		// Then go right if you hit a wall
		move = true;

		if (horizontalRayCast.IsColliding())
		{
			direction = -direction;
			horizontalRayCast.TargetPosition = -horizontalRayCast.TargetPosition;
		}

		// Drop on the player when above
		if (rayCast.IsColliding())
		{
			move = false;
			isFlying = false;
			startFlyingAttack = false;
			animPlayer.Play("emerge");
		}
	}

	private void DoJumpingAttack()
	{
		move = true;

		if (horizontalRayCast.IsColliding())
		{
			direction = -direction;
			horizontalRayCast.TargetPosition = -horizontalRayCast.TargetPosition;
			bodySprite.FlipH = !bodySprite.FlipH;
		}
	}

	private async void EndLevel()
	{
		if (!isLevelEnding) {
			isLevelEnding = true;
			GameManager.instance.canPause = false;

			animPlayer.Play("death");

			for (int i = 0; i < 6; i++) {
				audioComponent.playSFX("res://Sounds/SFX/explosion.wav", -15.0f);
				await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);
			}

			// Load stage clear music
			GameManager.instance.audio.Stream = (AudioStream)ResourceLoader.Load("res://Sounds/Music/StageClear.mp3");
			GameManager.instance.audio.VolumeDb = 0.0f;
			GameManager.instance.audio.Play();
			
			await ToSignal(GetTree().CreateTimer(4.5f), SceneTreeTimer.SignalName.Timeout);
			GameManager.instance.GoToMenu();
		}
	}
}
