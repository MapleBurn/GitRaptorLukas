using Godot;
using System;
using System.Threading;

public partial class World1 : Node2D
{
	//private Thread thread;
	[Export] public Node _generator;
	[Export] public Spawner _spawner;
	[Export] public Camera2D _camera;
	[Export] public Player _player;
	
	public override void _Ready()
	{
		_generator.Call("generate", null);
	}
	public override void _Process(double delta) { }

	public override void _Input(InputEvent @event)
	{
		//base._Input(@event);
		if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickLeft")) //input lze předělat na signály
		{
			Vector2 mousePos = GetGlobalMousePosition();
			if  (_spawner.isToggledPleb)
				_spawner.SpawnPleb(mousePos);
		}
		else if (@event is InputEventMouse && @event.IsAction("MouseWheelUp"))
		{
			if (_camera.Zoom.X <= 5){
				_camera.Zoom *= 1.25f;
				_player.Speed /= 1.25f;
			}
		}
		else if (@event is InputEventMouse && @event.IsAction("MouseWheelDown"))
		{
			if (_camera.Zoom.X >= 0.1f){
				_camera.Zoom /= 1.25f; 
				_player.Speed *= 1.25f;
			}
		}
	}
}
