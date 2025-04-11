using Godot;
using System;

public partial class GroundTiles : TileMapLayer
{
    [Export] private Spawner _spawner;
    private Rect2 _worldBounds;

    public void GenerationFinished()
    {
        UpdateWorldBounds();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && 
            mouseEvent.IsActionPressed("clickLeft"))
        {
            Vector2 mousePos = GetGlobalMousePosition();
            
            if (_worldBounds.HasPoint(mousePos) && _spawner.isToggledPleb)
            {
                _spawner.SpawnPleb(mousePos);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private void UpdateWorldBounds()
    {

        Rect2I usedRect = GetUsedRect();
        Vector2 tileSize = GetTileSet().GetTileSize();
        
        _worldBounds = new Rect2(
            position: new Vector2(0, 0),
            size: usedRect.Size * tileSize
        );
    }
}
