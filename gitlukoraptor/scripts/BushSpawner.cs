using Godot;
using System;

public partial class BushSpawner : Node2D
{
    [Export] public TileMapLayer GroundTiles;
    [Export] public TileMapLayer NatureTiles;
    private Random rnd = new Random();
    
    private void GenerationFinished()
    {
        foreach (Vector2I cellCoords in GroundTiles.GetUsedCells())
        {
            Vector2I cellData = GroundTiles.GetCellAtlasCoords(cellCoords);
            if (cellData != null && cellData == new Vector2I(2, 0))
            {
                if (rnd.Next(0, 100) <= 10)
                {
                    NatureTiles.SetCell(cellCoords, 0, new Vector2I(0, 0));
                }
            }
        }
    }
}
