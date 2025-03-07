using Godot;
using System;

public partial class Pleb : CharacterBody2D
{
	private Random rdm = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void MoveTime_Tick()
	{
		Position += new Vector2(rdm.Next(-20, 20), rdm.Next(-20, 20));
	}
}
