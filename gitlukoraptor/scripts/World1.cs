using Godot;
using System;
using System.Threading;

public partial class World1 : Node2D
{
	private Thread thread;
	private bool isToggledPleb = false;
	[Export] public Node _generator;
	[Export] public Spawner _spawner;
	
	public override void _Ready()
	{
		_generator.Call("generate", null);
	}
	public override void _Process(double delta) { }

	public override void _Input(InputEvent @event)
	{
		//base._Input(@event);
		if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickLeft"))
		{
			Vector2 mousePos = GetGlobalMousePosition();
			if  (isToggledPleb)
				_spawner.SpawnPleb(mousePos);
		}
	}

	private void btnPlebPressed()
	{
		isToggledPleb = !isToggledPleb;
		/*if (!isToggledPleb)
			return; */
	}
}
