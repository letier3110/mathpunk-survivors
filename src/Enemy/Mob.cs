using Godot;
using System;

public partial class Mob : RigidBody2D
{
	[Export]
	public double MinMobSpeed { get; set; } = 10.0;
	[Export]
	public double MaxMobSpeed { get; set; } = 15.0;
	[Export]
	public double Damage { get; set; } = 2.0;

	public override void _Ready()
	{
		AddToGroup("mobs");
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
		animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
