using Godot;
using System;

public partial class CameraController : Camera2D
{
    
    [Export] public Player _player;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            Vector2 mousePos = mouseEvent.Position;
            Vector2 viewportCenter = GetViewport().GetVisibleRect().Size / 2;
            Vector2 mouseOffset = mousePos - viewportCenter;

            if (mouseEvent.IsActionPressed("MouseWheelUp") && Zoom.X <= 5)
            {
                Vector2 mouseWorldPosPreZoom = Position + mouseOffset / Zoom;
                Zoom *= 1.25f;
                _player.Speed /= 1.25f;
                Position = mouseWorldPosPreZoom - mouseOffset / Zoom;
            }
            else if (mouseEvent.IsActionPressed("MouseWheelDown") && Zoom.X >= 0.1f)
            {
                Vector2 mouseWorldPosPreZoom = Position + mouseOffset / Zoom;
                Zoom /= 1.25f;
                _player.Speed *= 1.25f;
                Position = mouseWorldPosPreZoom - mouseOffset / Zoom;
            }
        }
    }
}
