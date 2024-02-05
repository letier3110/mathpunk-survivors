using Godot;
using System;

public partial class Bullet : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	private int Speed { get; set; } = 50;
	// Called when the node enters the scene tree for the first time.
	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.X * (float)(Speed * delta);
	}

	public override void _Ready()
	{
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("mobs"))
		{
			body.QueueFree();
		}
	}

	private void OnVisibilityScreenExited()
	{
		QueueFree();
	}
}
