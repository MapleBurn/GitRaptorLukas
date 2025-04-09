using Godot;
using System;

public partial class Pleb : CharacterBody2D
{
	private Random rdm = new Random();
	public Timer gameTimer;
	private Label lblDebug;
	
	//pleb stats
	public int health = 100;
	public int hunger = 100;
	public float speed = 50f;
	public Vector2 direction = Vector2.Zero;
	
	
	public override void _Ready()
	{
		gameTimer = GetNode<Timer>("/root/world1/GameTick");
		lblDebug = GetNode<Label>("Label");
		gameTimer.Timeout += () => gameTimer_Tick();
	}

	public override void _PhysicsProcess(double delta)
	{
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

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickRight"))
		{
			//GD.Print($"Clicked on Pleb: {Name} (Instance ID: {GetInstanceId()})");
			lblDebug.Visible = !lblDebug.Visible;
		}
	}

	private void Die()
	{
		QueueFree();
	}
	
}
