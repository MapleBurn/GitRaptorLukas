using Godot;
using System;

public partial class Spawner : Node2D
{ 
	private PackedScene _pleb;
	public bool isToggledPleb;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pleb = (PackedScene)GD.Load("res://scenes/pleb.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SpawnPleb(Vector2 pos)
	{
		Pleb pleb = (Pleb)_pleb.Instantiate();
		AddChild(pleb);
		pleb.ZIndex = 100;
		pleb.Position = pos;
	}
	
	private void BtnPlebPressed()
	{
		isToggledPleb = !isToggledPleb;
	}

	private void OnTimerTimeout() { }
}
