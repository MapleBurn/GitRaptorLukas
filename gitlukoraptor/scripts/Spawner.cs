using Godot;
using System;

public partial class Spawner : Node2D
{ 
	private PackedScene _pleb;
	public bool isToggledPleb;
	public static Rid navMap;
	private static bool initialized = false;
	
	public override void _Ready()
	{
		_pleb = (PackedScene)GD.Load("res://scenes/pleb.tscn");
	}
	
	public override void _Process(double delta)
	{
		
	}

	public static void Init(NavigationAgent2D anyAgent)
	{
		if (initialized) return;
		navMap = anyAgent.GetNavigationMap();
		initialized = true;
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
