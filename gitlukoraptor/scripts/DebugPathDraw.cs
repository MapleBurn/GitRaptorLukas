using Godot;
using System;
using GDC = Godot.Collections;

public partial class DebugPathDraw : Node2D
{
    private Pleb _pleb;
    
    public override void _Ready()
    {
        _pleb = GetParent() as Pleb;
        _pleb.Initialize();
    }
    public override void _Draw()
    {
        var path = _pleb.currentPath;
        if (path == null || path.Count == 0 || Pleb.map == null)
            return;

        int lastIndex = path.Count - 1;
        for (int i = 0; i < path.Count; i++)
        {
            // world‐space position of the tile center
            Vector2 pos = Pleb.map.MapToLocal(path[i]) + (Pleb.map.TileSet.TileSize / 2);

            // if this is the final point, draw it white
            Color col = (i == lastIndex) ? Colors.White : Colors.Red;
            DrawCircle(pos, 5f, col, true);
        }
    }

    public override void _Process(double delta)
    {
        QueueRedraw(); // constantly update the drawing
    }
}
