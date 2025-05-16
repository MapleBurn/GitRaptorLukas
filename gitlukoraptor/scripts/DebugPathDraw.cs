using Godot;
using System;
using GDC = Godot.Collections;

public partial class DebugPathDraw : Node2D
{
    public Pleb _pleb;

    private GDC.Array<Vector2I> currentPath;
    public override void _Ready()
    {
        _pleb = GetParent() as Pleb;
        _pleb.Initialize();
        currentPath = _pleb.currentPath;
    }
    public override void _Draw()
    {
        if (currentPath == null || currentPath.Count == 0 || Pleb.map == null)
            return;

        for (int i = 0; i < currentPath.Count; i++)
        {
            // Convert tile coords to world/global space
            Vector2 worldPos = Pleb.map.MapToLocal(currentPath[i]);

            // Optional: offset to center the circle
            worldPos += Pleb.map.TileSet.TileSize / 2;

            DrawCircle(worldPos, 5f, Colors.Red);
        }
    }

    public override void _Process(double delta)
    {
        QueueRedraw(); // constantly update the drawing
    }
}
