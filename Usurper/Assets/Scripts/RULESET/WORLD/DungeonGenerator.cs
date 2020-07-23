using System.Collections.Generic;
using UnityEngine;

namespace RULESET.WORLD
{
    public enum TILE_TYPES
    {
        WALL = 256,
        FLOOR = 257,
        DOOR = 258,
        ENTRANCE = 259
    }

    public class DungeonGenerator
    {
        public enum SIZE_PRESETS_NAMES
        {
            TINY,
            SMALL,
            DEFAULT,
            LARGE,
            HUGE
        }

        public enum ALGORITHM_NAMES
        {
            PATHFINDER,
        }


        public int width;
        public int height;

        public DungeonAlgorithm[] algorithms =
            {
                new PathfinderAlgorithm()
            };

        public Vector2Int[] sizePresets =
            {
                new Vector2Int(30, 20),
                new Vector2Int(50, 30),
                new Vector2Int(75, 50),
                new Vector2Int(100, 80),
                new Vector2Int(130, 100)
            };

        public int[,] GenerateDungeon(ALGORITHM_NAMES algorithm, SIZE_PRESETS_NAMES sizePresetIndex)
        {
            Vector2Int size = sizePresets[(int)sizePresetIndex];
            width = size.x;
            height = size.y;
            return algorithms[(int)algorithm].Generate(size.x, size.y);
        }
    }

    public abstract class DungeonAlgorithm
    {
        public TILE_TYPES[,] Tiles;

        public abstract int[,] Generate(int width, int height);   

        public void CreateCanvas(int width, int height)
        {
            Tiles = new TILE_TYPES[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Tiles[x, y] = TILE_TYPES.WALL;
        }

        public int[,] ConvertToIntData(int width, int height)
        {
            int[,] tileData = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    tileData[x, y] = (int)Tiles[x, y];

            return tileData;
        }
    }
}