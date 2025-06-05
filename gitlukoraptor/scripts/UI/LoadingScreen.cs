using Godot;
using System;

public partial class LoadingScreen : Control
{
	private Godot.Collections.Array progress;
	private string sceneName;
	private ProgressBar progressBar;
	public override void _Ready()
	{
		sceneName = "res://scenes/world_1.tscn";
		ResourceLoader.LoadThreadedRequest(sceneName);
		progressBar = GetNode<ProgressBar>("ProgressBar");
		progress = new Godot.Collections.Array();
	}
	
	public override void _Process(double delta)
	{
		var loadStatus = ResourceLoader.LoadThreadedGetStatus(sceneName, progress);
		progressBar.Value = (double)progress[0] * 100;
		if (loadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
		{
			PackedScene world1 = (PackedScene)ResourceLoader.LoadThreadedGet(sceneName);
			GetTree().ChangeSceneToPacked(world1);
		}
		else if (loadStatus == ResourceLoader.ThreadLoadStatus.Failed)
		{
			GD.Print("Loading World1 failed!");
		}
	}
}
