using Godot;
using System;

public partial class MainMenu : Control
{
	
	public override void _Ready() {}
	public override void _Process(double delta) {}

	private void OnButtonPressed()
	{
		PackedScene loadingScreen = (PackedScene)GD.Load("res://scenes/loading_screen.tscn");
		GetTree().ChangeSceneToPacked(loadingScreen);
	}
}
