using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
	public void OnExitPresed() {
		GetTree().Quit();
	}

	public void OnStartPressed() {
		MainProcess mainProcess = GetNodeOrNull<MainProcess>("/root/MainProcess");
		if(mainProcess == null) {
			GD.Print("Error. Could not find MainProcess");
			return;
		}
		mainProcess.ChangeScene("res://scenes/MainGame.tscn", true);
	}
}
