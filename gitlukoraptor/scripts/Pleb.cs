using Godot;
using System;

public partial class Pleb : CharacterBody2D
{
	private Random rdm = new Random();
	private const float Speed = 50f;
	private Timer timer;
	private Vector2 direction = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		timer = GetNode<Timer>("moveTime");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void MoveTime_Tick()
	{
		//direction = new Vector2((int)(rdm.Next(-100, 100) / 100), (int)(rdm.Next(-100, 100) / 100));
		timer.WaitTime = (double)rdm.Next(1, 5) / 10;
	}
}
