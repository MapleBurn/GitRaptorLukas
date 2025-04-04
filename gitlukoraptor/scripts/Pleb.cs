using Godot;
using System;

public partial class Pleb : CharacterBody2D
{
	private Random rdm = new Random();
	private const float Speed = 50f;
	private Vector2 direction = Vector2.Zero;
	public Timer gameTimer;
	private Label lblDebug;
	
	//pleb stats
	private int health = 100;
	private int hunger = 100;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameTimer = GetNode<Timer>("/root/world1/GameTick");
		lblDebug = GetNode<Label>("Label");
		gameTimer.Timeout += () => gameTimer_Tick();
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
	private void gameTimer_Tick()
	{
		if (health <= 0)
		{
			Die();
			return;
		}
		if  (hunger <= 0)
			health--;
		else
			hunger--;
		
		lblDebug.Text = "Hunger: " + hunger + "\nHealth: " + health;
	}

	private void Die()
	{
		QueueFree();
	}
	
}
