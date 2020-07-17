using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using RENDERER.UTILS.Atlas;
using RULESET.WORLD;

namespace RENDERER.MAP
{
    public class MapDungeonFlatRenderer : MonoBehaviour
    {
        //This is a map offset to get a minimum size where at least one room will be created!
        private const int MAX_ROOMS_IN_DUNGEON = 128;
        private const int MAX_ENTITIES_IN_DUNGEON = 96;
        
        
        public int width =  12;
        public int height = 12;

        public int minRoomSize = 3;
        public int maxRoomSize = 8;

        private DungeonGenerator _dungeonGenerator = new DungeonGenerator();
        public int[,] mapData;
        public Tilemap tilemap;

        //This needs a total rework to instead use the cell based implementation!
        //The cells are placed at 2x+1, 2y+1 with the 0 row and column filled with wall tiles!

        private void Start()
        {
            mapData = _dungeonGenerator.GenerateDungeon(4,4);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                DrawFullMap();
            }
        }

        public void DrawFullMap()
        {
            //Here we convert the int mapData into tiles for the editor view.
            //How do we store and access the dungeon resources? Do we split the Sprite Atlas? (Yes. Access the Dungeon Sprite List)
            List<TileObject> cachedTiles = new List<TileObject>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    //For each int in mapData, map it to a tile & draw it to the tilemap
                    //We have to contact the dungeon tile editor...
                    foreach (var cached in cachedTiles)
                    {
                        if (cached.id == mapData[x, y])
                        {
                            //If we find a cached version of the tile, skip loading in a new one!
                            goto EndOfLoop;
                        }
                    }
                    
                    //Since we couldn't we find a cached version of this id, we fetch a new one and add it to the cache.
                    cachedTiles.Add(TileAtlas.FetchDungeonTileObjectById(mapData[x, y]));
                    
                    EndOfLoop:
                        // Here is where we render the respective tile to the tilemap!
                        foreach (var cached in cachedTiles)
                        {
                            if (cached.id == mapData[x, y])
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), cached.tile);
                            }
                        }
                        continue;
                }
            
            Debug.Log("cached tile count: " + cachedTiles.Count);
        }
    }
}