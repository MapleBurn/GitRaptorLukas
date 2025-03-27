using Godot;
using System;
using System.Threading;

public partial class World1 : Node2D
{
	private Thread thread;

	[Export] public Node _generator;

	public override void _Ready()
	{
		_generator.Call("generate", null);
	}

	public override void _Process(double delta)
	{

	}

	private void genNoise()
	{
	}
}
