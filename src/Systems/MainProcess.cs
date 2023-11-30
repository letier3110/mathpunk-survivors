using System;
using Godot;

public partial class MainProcess : CanvasLayer
{
	public static MainProcess Instance { get; private set; }
	public ResourceLoader.ThreadLoadStatus LoadingStatus { get; set; }
	private string LatestScenePath { get; set; }
	private Node LatestNode { get; set; }
	private string _mainMenuScenePath = "res://scenes/MainMenu.tscn";
	private string _mainGameScenePath = "res://scenes/MainGame.tscn";
	public override void _Ready()
	{
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		ChangeScene(_mainMenuScenePath);
	}

	public void ChangeScene(string scenePath, Boolean removeOldScene = false)
	{
		GetNode<CanvasLayer>("StartSplash").Show();
		if (removeOldScene && LatestNode != null)
		{
			LatestNode.QueueFree();
		}
		LatestScenePath = scenePath;
		try
		{
			ResourceLoader.LoadThreadedRequest(scenePath);
			LoadingStatus = ResourceLoader.ThreadLoadStatus.InProgress;
		}
		catch (System.Exception e)
		{
			GD.Print(e);
		}
	}

	public override void _Process(double delta)
	{
		LoadingStatus = ResourceLoader.LoadThreadedGetStatus(LatestScenePath);
		switch (LoadingStatus)
		{
			case ResourceLoader.ThreadLoadStatus.InProgress:
				// progress_bar.value = progress[0] * 100 # Change the ProgressBar value
				break;
			case ResourceLoader.ThreadLoadStatus.Loaded:
				// When done loading, change to the target scene:
				var resScene = ResourceLoader.LoadThreadedGet(LatestScenePath);
				if (resScene == null)
				{
					GD.Print("Error. Could not load Resource");
					return;
				}
				PackedScene packedScene = resScene as PackedScene;
				if (packedScene != null)
				{
					Node scene = packedScene.Instantiate();
					LatestNode = scene;
					AddChild(scene);
					GetNode<CanvasLayer>("StartSplash").Hide();
				}
				break;
			case ResourceLoader.ThreadLoadStatus.Failed:
				// Well some error happend:
				GD.Print("Error. Could not load Resource");
				break;
			default:
				break;
		}
	}
}
