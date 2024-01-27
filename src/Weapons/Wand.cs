using Godot;
using System;

public partial class Wand : Node
{
	[Export]
	public double ShootRate { get; set; } = 1.5;
	[Export]
	public PackedScene Bullet { get; set; }

	private void OnShootTimeout()
	{
		Shoot();
	}

	public void Shoot()
	{
		if (this == null) return;
		Node2D parentNode = GetParentOrNull<Node2D>();
		if (parentNode == null) return;
		try
		{
			Godot.Collections.Array<Node> mobs = GetTree().GetNodesInGroup("mobs");
			if (mobs.Count == 0) return;
			long randomMob = GD.Randi() % mobs.Count;
			Node mob = mobs[(int)randomMob];
			if (mob == null) return;
			Node2D position = (Node2D)mob;
			if (position == null) return;
			Bullet b = Bullet.Instantiate<Bullet>();
			// GetTree().Root.FindChild("Main").AddChild(b);
			// GetTree().Root.GetChild(1).AddChild(b);
			// GetParentOrNull<Node>().GetParentOrNull<Node>();
			// GetTree().Root.AddChild(b);
			GetParentOrNull<Node>().GetParentOrNull<Node>().AddChild(b);
			// GetNode<MainGame>("/MainProcess/Main").AddChild(b);
			b.Transform = parentNode.GlobalTransform;
			Vector2 direction = (position.Position - parentNode.Position).Normalized();
			b.Rotate(direction.Angle());
		}
		catch (Exception e)
		{
			GD.Print(e);
		}
	}

	public void InitWand()
	{
		Timer shootTimer = GetNodeOrNull<Timer>("ShootTimer");
		if (shootTimer == null) return;
		shootTimer.WaitTime = 1 / ShootRate;
		shootTimer.Timeout += OnShootTimeout;
		shootTimer.Start();
	}

	public override void _Ready()
	{
		InitWand();
	}
}
