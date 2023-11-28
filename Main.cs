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
		/*
			NB
			Instancing in Godot is handled using the MultiMeshInstance node. 
			It's the instanced counterpart to MeshInstance. See 
			[link: https://docs.godotengine.org/en/latest/tutorials/performance/using_multimesh.html ] Optimization 
			using MultiMeshes in the documentation for more information.
			Keep in mind MultiMeshes aren't suited if you need to move the objects in different 
			directions every frame (although you can can achieve this by using INSTANCE_ID in a shader 
			shared among all instances). MultiMeshInstance lets you change how many instances are visible 
			by setting its visible_instance_count property.
		*/
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
		GetNode<GUI>("GUI").GameOver();
		// GetNode<Timer>("ScoreTimer").Stop();
	}

	public void NewGame()
	{
		_score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");

		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
		GetNode<GUI>("GUI").GameStart();
	}

	public override void _Ready()
	{
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		NewGame();
	}
}
