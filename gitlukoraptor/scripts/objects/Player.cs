using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float defaultSpeed = 300.0f;
	public float Speed;

	private bool _isDragging;
	private Vector2 _previousMousePosition;

	public override void _Ready()
	{
		Speed = defaultSpeed;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsAction("clickMiddle"))
			{
				if (mouseEvent.Pressed)
				{
					_isDragging = true;
					_previousMousePosition = mouseEvent.Position;
				}
				else
				{
					_isDragging = false;
				}
			}
		}
		if (@event is InputEventMouseMotion motionEvent && _isDragging)
		{
			Vector2 delta = _previousMousePosition - motionEvent.Position;
			Position += delta * (Speed / defaultSpeed);
			_previousMousePosition = motionEvent.Position;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
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
}
