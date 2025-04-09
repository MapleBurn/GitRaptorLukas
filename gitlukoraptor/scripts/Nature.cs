using Godot;
using System;

namespace GitLukoraptor.scripts;

public class Nature
{
    private static int[][] _bushHeatMap;
    private static int[][] _spawnedBush;

    private const int BUSH_PRINT_SIZE = 10;
    
    static Nature()
    {
        _bushHeatMap = new int[512][];
        for (int i = 0; i < 512; i++)
        {
            _bushHeatMap[i] = new int[512];
        }

        _spawnedBush = CreateBushPrint(BUSH_PRINT_SIZE);
    }

    public static bool AllowBushSpawn(Vector2I position)
    {
        return _bushHeatMap[position.Y][position.X] < 5;
    }

    public static void SpawnBush(Vector2I position)
    {
        for (int i = 0; i < _spawnedBush.Length; i++)
        {
            for (int j = 0; j < _spawnedBush.Length; j++)
            {
                _bushHeatMap[position.Y + j - BUSH_PRINT_SIZE/2][position.X + i - BUSH_PRINT_SIZE/2] += _spawnedBush[i][j];
                if (_bushHeatMap[position.Y + j - BUSH_PRINT_SIZE / 2][position.X + i - BUSH_PRINT_SIZE / 2] > 100)
                    _bushHeatMap[position.Y + j - BUSH_PRINT_SIZE / 2][position.X + i - BUSH_PRINT_SIZE / 2] = 100;
            }
        }

        
        
    }
    
    private static int[][] CreateBushPrint(int size)
    {
        int[][] array = new int[size][];
        for (int i = 0; i < size; i++)
        {
            array[i] = new int[size];
        }

        double centerX = (double)(size - 1) / 2.0;
        double centerY = (double)(size - 1) / 2.0;
        double maxRadius = (double)size / 2.0;

        const int centerValue = 100;
        const int edgeValue = 5;
        const double valueRange = centerValue - edgeValue;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                double distance = Math.Sqrt(Math.Pow(i - centerY, 2) + Math.Pow(j - centerX, 2));
                double normalizedDistance = (maxRadius > 0) ? Math.Min(1.0, distance / maxRadius) : 0;
                double calculatedValue = centerValue - (valueRange * normalizedDistance);
                array[i][j] = Math.Max(edgeValue, Math.Min(centerValue, (int)Math.Round(calculatedValue)));
            }
        }
        if (size > 0)
        {
             array[size / 2][size / 2] = centerValue;
        }
        return array;
    }
}