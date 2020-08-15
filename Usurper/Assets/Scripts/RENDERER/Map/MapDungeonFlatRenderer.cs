using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using RENDERER.UTILS.Atlas;
using RULESET.WORLD;
using RULESET.MANAGERS;

namespace RENDERER.MAP
{
    public class MapDungeonFlatRenderer : MonoBehaviour
    {
        //This is a map offset to get a minimum size where at least one room will be created!

        public static Gate selected;
        private readonly DungeonGenerator _dungeonGenerator = new DungeonGenerator();
        public Tilemap tilemap;

        private int currentAlgorithm = (int)DungeonGenerator.ALGORITHM_NAMES.PATHFINDER;
        private int currentSize = (int) DungeonGenerator.SIZE_PRESETS_NAMES.SMALL;

        //This needs a total rework to instead use the cell based implementation!
        //The cells are placed at 2x+1, 2y+1 with the 0 row and column filled with wall tiles!

        public void SetAlgorithm(int index)
        {
            currentAlgorithm = index;
        }

        public void SetSize(int index)
        {
            currentSize = index;
        }

        public void SetSelectedGateName(string name)
        {
            if (selected.dngData == null) return;
            selected.name = name;
            // Call for the UI to update to reflect the new name
        }
        
        public void GenerateDungeonWithSettings()
        {
            if (selected.dngData == null) return;
            selected.dngData = _dungeonGenerator.GenerateDungeon((DungeonGenerator.ALGORITHM_NAMES)currentAlgorithm, (DungeonGenerator.SIZE_PRESETS_NAMES)currentSize);
            selected.width = _dungeonGenerator.width;
            selected.height = _dungeonGenerator.height;

            for (int i = 0; i < GateManager.Gates.Count; i++)
            {
                if (selected.x == GateManager.Gates[i].x && selected.y == GateManager.Gates[i].y)
                {
                    Debug.Log("Found Match with the selected gate");
                    GateManager.Gates[i] = selected;
                }
            }

            DrawFullMap();
        }

        public void DrawFullMap()
        {
            //Here we convert the int mapData into tiles for the editor view.
            //How do we store and access the dungeon resources? Do we split the Sprite Atlas? (Yes. Access the Dungeon Sprite List)
            if (selected.dngData == null) return;

            tilemap.ClearAllTiles();
            List<TileObject> cachedTiles = new List<TileObject>();
            for (int y = 0; y < selected.height; y++)
                for (int x = 0; x < selected.width; x++)
                {
                    //For each int in mapData, map it to a tile & draw it to the tilemap
                    //We have to contact the dungeon tile editor...
                    foreach (var cached in cachedTiles)
                    {
                        if (cached.id == selected.dngData[x, y])
                        {
                            //If we find a cached version of the tile, skip loading in a new one!
                            goto EndOfLoop;
                        }
                    }
                    
                    //Since we couldn't we find a cached version of this id, we fetch a new one and add it to the cache.
                    cachedTiles.Add(TileAtlas.FetchDungeonTileObjectById(selected.dngData[x, y]));
                    
                    EndOfLoop:
                        // Here is where we render the respective tile to the tilemap!
                        foreach (var cached in cachedTiles)
                        {
                            if (cached.id == selected.dngData[x, y])
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), cached.tile);
                            }
                        }
                        continue;
                }
        }
    }
}