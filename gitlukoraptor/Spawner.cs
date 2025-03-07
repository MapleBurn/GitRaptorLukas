using Godot;
using System;

public partial class Spawner : Node2D
{ 
	private PackedScene _pleb;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pleb = (PackedScene)GD.Load("res://pleb.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void OnTimerTimeout()
	{
		Pleb pleb = (Pleb)_pleb.Instantiate();
		this.AddChild(pleb);
		pleb.Position = new Vector2(300, 300);
	}
}
