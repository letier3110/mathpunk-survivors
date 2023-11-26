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
		Mob mob = MobScene.Instantiate<Mob>();
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		mob.Position = mobSpawnLocation.Position;
		var velocity = new Vector2((float)GD.RandRange(mob.MinMobSpeed, mob.MaxMobSpeed), 0);
		var direction = (player.Position - mob.Position).Normalized();
		mob.LinearVelocity = velocity.Rotated(direction.Angle());
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
