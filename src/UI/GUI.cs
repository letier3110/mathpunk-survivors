using Godot;
using System;

public partial class GUI : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void StartGameEventHandler();

	private int timePassed = 0;

	public void ShowMessage(string text)
	{
		Label node = GetNode<Label>("Time");
		node.Text = text;
	}

	private string FormatTime(int time)
	{
		int minutes = time / 60;
		int seconds = time - minutes * 60;
		return string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	public void GameStart()
	{
		timePassed = 0;
		string newTime = FormatTime(timePassed);
		ShowMessage(newTime);
		GetNode<Timer>("Timer").Start();
	}

	public void GameOver()
	{
		// timePassed = 0;
		// string newTime = FormatTime(timePassed);
		ShowMessage("Game Over!");
		GetNode<Timer>("Timer").Stop();
	}

	private void OnMessageTimerTimeout()
	{
		// GetNode<Label>("Message").Hide();
		timePassed++;
		string newTime = FormatTime(timePassed);
		ShowMessage(newTime);
	}
}
