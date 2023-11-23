using Godot;
using System;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public PackedScene MobScene { get; set; }
	private int _score;

	private void OnScoreTimerTimeout()
	{
		_score++;
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		// GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnMobTimerTimeout()
	{
		var player = GetNode<Player>("Player");
		if(player == null) return;

		// Note: Normally it is best to use explicit types rather than the `var`
		// keyword. However, var is acceptable to use here because the types are
		// obviously Mob and PathFollow2D, since they appear later on the line.

		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		mob.Position = mobSpawnLocation.Position;
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		// direction to player position
		var direction = (player.Position - mob.Position).Normalized();
		// set velocity from direction to radians
		mob.LinearVelocity = velocity.Rotated(direction.Angle());

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		// GetNode<Timer>("ScoreTimer").Stop();
	}

	public void NewGame()
	{
		_score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");

		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
	}

	public override void _Ready()
	{
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		NewGame();
	}
}
