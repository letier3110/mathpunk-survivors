using Godot;
using System;

// var globalWand = GD.Load<PackedScene>("res://src/Weapons/Wand.tscn");

public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public double HitPoints { get; set; } = 100;
	[Export]
	public double MaxHitPoints { get; set; } = 100;
	[Export]
	public double RegenRate { get; set; } = 0.1;
	[Export]
	public int Speed { get; set; } = 100;

	public Vector2 ScreenSize; // Size of the game window.

	private Wand wand;

	// TODO: rewrite from player receive damage by collision to Monster doing damage to player
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("mobs"))
		{
			Mob mob = body as Mob;
			HitPoints -= mob.Damage;
			if (HitPoints <= 0)
			{
				HitPoints = 0;
				Hide();
				DestroyInventory();
				var hpProgress = GetNode<Control>("HealthBar");
				hpProgress.Hide();
				EmitSignal(SignalName.Hit);
				GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
		}
	}

	public void Start(Vector2 position)
	{
		InitInventory();
		Position = position;
		Show();
		var hpProgress = GetNode<Control>("HealthBar");
		hpProgress.Show();
		DrawHp();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	public void DrawHp()
	{
		// var hp = GetNode<Control>("Canvas/HitpointsBar");
		// hp.Size = new Vector2((float)HitPoints, 15);

		// progress bar draw
		// var hpProgress = GetNode<ProgressBar>("HpProgress");
		// hpProgress.Value = hpProgress.MaxValue * (float)(HitPoints / MaxHitPoints);

		// progress bar draw
		var hpProgress = GetNode<TextureProgressBar>("HealthBar/TextureProgressBar");
		hpProgress.Value = hpProgress.MaxValue * (float)(HitPoints / MaxHitPoints);

		// custom draw on empty sprite
		// var hpSprite = GetNode<Sprite2D>("HpBox");
		// hpSprite.DrawRect(new Rect2(0, 0, (float)HitPoints, 15), new Color(1, 0, 0));
		// hpSprite.DrawRect(new Rect2((float)HitPoints, 0, (float)MaxHitPoints - (float)HitPoints, 15), new Color(0, 0, 0));
	}

	private void HandleInput(double delta) {
		var direc = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		var v = direc.Normalized() * Speed;
		// Velocity = direc.Normalized() * Speed;
		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}
		Velocity = velocity;
		// Position += v * (float)delta;
		// Position += velocity * (float)delta;
		// Position = new Vector2(
		// 		x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
		// 		y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		// );
	}

	private void UpdateVelocity() {
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (Velocity.Y < 0)
		{
			animatedSprite2D.Animation = "back";
		}
		else if (Velocity.Y > 0)
		{
			animatedSprite2D.Animation = "forward";
		}
		else if (Velocity.X == 0 && Velocity.Y == 0)
		{
			animatedSprite2D.Animation = "idle";
		}
		MoveAndCollide(Velocity);
	}

	private void UpdateMobsDirection() {
		Godot.Collections.Array<Node> mobs = GetTree().GetNodesInGroup("mobs");
		if (mobs.Count == 0) return;
		// if(Velocity.X == 0 && Velocity.Y == 0) return;
		foreach (Node mob in mobs)
		{
			if (mob == null) return;
			Mob m = mob as Mob;
			if (m == null) return;
			var velocity = new Vector2(m.ActualSpeed, 0);
			var direction = (Position - m.Position).Normalized();
			m.LinearVelocity = velocity.Rotated(direction.Angle());
		}
	}

	private void UpdatePlayerStats(double delta) {
		if (HitPoints < MaxHitPoints)
		{
			HitPoints += RegenRate * delta;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// HandleInput(delta);
		UpdateVelocity();
		UpdateMobsDirection();
		UpdatePlayerStats(delta);
		// DrawHp();
	}


	public override void _Process(double delta)
	{
		HandleInput(delta);
		// UpdateVelocity();
		// UpdatePlayerStats(delta);
		DrawHp();
	}

	public void InitInventory()
	{
		var globalWand = GD.Load<PackedScene>("res://scenes/Wand.tscn");
		var wand = globalWand.Instantiate<Wand>();
		AddChild(wand);
	}

	public void DestroyInventory()
	{
		if (wand != null) wand.QueueFree();
	}

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}
}