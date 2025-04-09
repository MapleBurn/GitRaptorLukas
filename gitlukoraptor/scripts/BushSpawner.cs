using Godot;
using System;
using GitLukoraptor.scripts;
using Environment = Godot.Environment;

public partial class BushSpawner : Node2D
{
	[Export] public TileMapLayer GroundTiles;
	private PackedScene _bush;
	private Random rnd = new Random();
	
	private void GenerationFinished()
	{
		_bush = GD.Load<PackedScene>("res://scenes/bush.tscn");
		
		foreach (Vector2I cellCoords in GroundTiles.GetUsedCells())
		{
			Vector2I cellData = GroundTiles.GetCellAtlasCoords(cellCoords);
			if (cellData == new Vector2I(2, 0))
			{
				if (rnd.Next(0, 100) <= 10 && IsSurroundingField(cellCoords))
				{
					if (Nature.AllowBushSpawn(cellCoords))
					{
						Bush bush = (Bush)_bush.Instantiate();
						AddChild(bush);
						bush.Position = (cellCoords * 16) + new Vector2I(8, 8);
						Nature.SpawnBush(cellCoords);
					}
				}
			}
		}
	}

	private bool IsSurroundingField(Vector2I coords)
	{
		var around = new Vector2I[] { new Vector2I(-1,-1), new Vector2I(0,-1),new Vector2I(1,-1),new Vector2I(-1,0),new Vector2I(1,0),new Vector2I(-1,1),new Vector2I(0,1),new Vector2I(1,1) };
		foreach (Vector2I coord in around)
		{
			if (GetTileType(coords + coord) != "field")
				return false;
		}
		return true;
	}
	
	private string GetTileType(Vector2I coords)
	{
		Vector2I cellData = GroundTiles.GetCellAtlasCoords(coords);
		if (cellData == new Vector2I(0, 0))
			return "shallow";
		if (cellData == new Vector2I(1, 0))
			return "water";
		if (cellData == new Vector2I(2, 0))
			return "field";
		if (cellData == new Vector2I(3, 0))
			return "forest";
		if (cellData == new Vector2I(4, 0))
			return "sand";
		if (cellData == new Vector2I(5, 0))
			return "mountain";
		return "error";
	}
}
