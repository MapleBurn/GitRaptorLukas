using Godot;
using System;

public partial class Tutorial : Node2D
{
	//[Export]
	//private PackedScene timer {  get; set; } 
	private int time = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnTimerTimeout()
	{
		time++;
		if (time > 2)
		{
			GetTree().ChangeSceneToFile("res://world_1.tscn");
		}
	}
}
