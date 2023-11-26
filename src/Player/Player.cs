using Godot;
using System;

public partial class Player : Area2D
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
	public int Speed { get; set; } = 400;
	[Export]
	public PackedScene Bullet { get; set; }

	public Vector2 ScreenSize; // Size of the game window.

	private void OnShootTimeout()
	{
		Shoot();
	}

	private void OnBodyEntered(Node2D body)
	{
		// Hide();
		if(body.IsInGroup("mobs"))
		{
			Mob mob = body as Mob;
			HitPoints -= mob.Damage;
			if(HitPoints <= 0)
			{
				HitPoints = 0;
				Hide();
				var hpProgress = GetNode<Control>("HealthBar");
				hpProgress.Hide();
				EmitSignal(SignalName.Hit);
				GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				GetNode<Timer>("ShootTimer").Stop();
			}
		}
		// EmitSignal(SignalName.Hit);
		// GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		// GetNode<Timer>("ShootTimer").Stop();
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		var hpProgress = GetNode<Control>("HealthBar");
		hpProgress.Show();
		DrawHp();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		GetNode<Timer>("ShootTimer").Start();
	}

	public void Shoot()
	{
		if(this == null) return;
		try {
			Godot.Collections.Array<Node> mobs = GetTree().GetNodesInGroup("mobs");
			if(mobs.Count == 0) return;
			var randomMob = GD.Randi() % mobs.Count;
			var mob = mobs[(int)randomMob];
			if(mob == null) return;
			var position = (Node2D)mob;
			if(position == null) return;
			Bullet b = Bullet.Instantiate<Bullet>();
			Owner.AddChild(b);
			b.Transform = GlobalTransform;
			var direction = (position.Position - Position).Normalized();
			b.Rotate(direction.Angle());
		} catch (Exception e) {
			GD.Print(e);
		}
	}

	public void DrawHp() {
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


	public override void _Process(double delta)
	{
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
		Position += velocity * (float)delta;
		Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
		if (velocity.Y < 0)
		{
			animatedSprite2D.Animation = "back";
			// animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			// animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y > 0)
		{
			animatedSprite2D.Animation = "forward";
			// animatedSprite2D.FlipV = velocity.Y > 0;
		}
		else if (velocity.X == 0 && velocity.Y == 0)
		{
			animatedSprite2D.Animation = "idle";
		}
		if (HitPoints < MaxHitPoints)
		{
			HitPoints += RegenRate * delta;
		}
		DrawHp();
	}

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}
}