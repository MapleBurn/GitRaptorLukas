using Godot;
using System;

public partial class Spawner : Node2D
{ 
	private PackedScene _pleb;
	public bool isToggledPleb;

	private bool onceSpawned = false;
	
	public override void _Ready()
	{
		_pleb = (PackedScene)GD.Load("res://scenes/pleb.tscn");
	}
	
	public override void _Process(double delta)
	{
		
	}

	public void SpawnPleb(Vector2 pos)
	{
		Pleb pleb = (Pleb)_pleb.Instantiate();
		AddChild(pleb);
		pleb.ZIndex = 100;
		pleb.Position = pos;
		if (!onceSpawned)
		{
			onceSpawned = true;
			Pleb.navMap = pleb.navAgent.GetNavigationMap();
		}
	}
	
	private void BtnPlebPressed()
	{
		isToggledPleb = !isToggledPleb;
	}

	private void OnTimerTimeout() { }
}
