using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

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
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		GetNode<Timer>("ShootTimer").Stop();
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
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
			Bullet b = Bullet.Instantiate<Bullet>();
			Owner.AddChild(b);
			b.Transform = this.GlobalTransform;
			var position = (Node2D)mob;
			// GD.Print(position.Position);
			// b.Transform.Scale = new Vector2(0.5f, 0.5f);
			var direction = (position.Position - Position).Normalized();
			b.Rotate(direction.Angle());
		} catch (Exception e) {
			GD.Print(e);
		}
	}


	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.
		// if (Input.IsActionPressed("shoot"))
		// {
		// 	Shoot();
		// }

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
	}

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}
}