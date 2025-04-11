using Godot;
using System;

public partial class Pleb : CharacterBody2D, Entity
{
	private Timer gameTimer;
	public Vector2 direction = Vector2.Zero;
	public float speed = 50f;
	
	//pleb data
	public Sprite2D sprite;
	public int health = 100;
	public string name = "Pleb";
	public bool favorite = false;
	
	public int hunger = 100;
	public string teamId = "none";
	
	
	public bool showDetails;
	DetailPopup detailPopup;
	
	public override void _Ready()
	{
		gameTimer = GetNode<Timer>("/root/world1/GameTick");
		sprite = GetNode<Sprite2D>("Sprite2D");
		detailPopup = GetNode<DetailPopup>("/root/world1/Hud/DetailPopup");
		
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

		if (showDetails)
		{
			detailPopup.Update();
		}
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("clickRight"))
		{
			showDetails = true;
			detailPopup.Display(this);
		}
	}

	public void Die()
	{
		QueueFree();
	}
	
}
