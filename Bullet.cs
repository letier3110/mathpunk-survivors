using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export]
	private int Speed { get; set; } = 100;
	// Called when the node enters the scene tree for the first time.
	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.X * (float)(Speed * delta);
	}

	private void OnBodyEntered(Node2D body)
	{
		// print all groups
		GD.Print(body.GetGroups());
		if (body.IsInGroup("mobs"))
		{
			body.QueueFree();
		}
		QueueFree();
		// EmitSignal(SignalName.Hit);
		// GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	private void OnVisibilityScreenExited()
	{
		QueueFree();
	}
}
