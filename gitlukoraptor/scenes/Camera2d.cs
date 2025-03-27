using Godot;
using System;

public partial class Camera2d : Camera2D
{
	public override void _Ready() { }
	public override void _Process(double delta) { }

	private void PlayerInputEvent(InputEvent inputEvent)
	{
		if (inputEvent.IsAction("MouseWheelUp"))
		{
			if (Zoom.X >= 5)
				return;
			else
				Zoom += new Vector2(1, 1);
		}
		else if (inputEvent.IsAction("MouseWheelDown"))
		{
			if (Zoom.X <= 1)
				return;
			else
				Zoom -= new Vector2(1, 1);
		}
	}
}
