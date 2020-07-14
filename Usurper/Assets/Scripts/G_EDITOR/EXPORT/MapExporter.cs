using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;
using RULESET.MANAGERS;
using LitJson;
using RENDERER.UTILS;

namespace EDITOR.EXPORT
{

    public class MapExporter : MonoBehaviour
    {
        private string campaignPath = Application.streamingAssetsPath + "/Campaigns/";
        private const string mapExtention = ".map";

        
        public StreamingResourceLoader resourceLoader;
        
        ChunkExporter chnkExporter = new ChunkExporter();
        ItemExporter itemExporter = new ItemExporter();


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartCoroutine(SaveMap());
            }
        }

        public bool LoadMap(string mapName)
        {
            if (!File.Exists(campaignPath + mapName + "/" + mapName + mapExtention)) return false;
            MapViewport mapObject = FindObjectOfType<MapViewport>();
            Debug.Log(campaignPath + mapName);
            JsonData loadedFile = JsonMapper.ToObject(File.ReadAllText(campaignPath + mapName + "/" + mapName + mapExtention));

            World.worldName = loadedFile["mapName"].ToString();
            Debug.Log(loadedFile["author"].ToString());
            World.width = (int)loadedFile["mapWidth"];
            World.height = (int)loadedFile["mapHeight"];
            
            resourceLoader.Init((int)loadedFile["spriteSize"], (int)loadedFile["dungeonSpriteSize"]);
            mapObject.centerPosOnMap = new Vector2Int((int)loadedFile["playerStartPosX"], (int)loadedFile["playerStartPosY"]);
            FindObjectOfType<EntityManager>().playerEntity.x = mapObject.centerPosOnMap.x;
            FindObjectOfType<EntityManager>().playerEntity.y = mapObject.centerPosOnMap.y;
            List<string> chunkPaths = new List<string>();
            for (int i = 0; i < loadedFile["chnkPaths"].Count; i++)
            {
                chunkPaths.Add(loadedFile["chnkPaths"][i].ToString());
            }

            List<Chunk> toLoad = new List<Chunk>();
            List<int[,]> loadedIntData = chnkExporter.LoadChunks(chunkPaths, mapName);
            int x = 0;
            int y = 0;
            foreach (var loadedData in loadedIntData)
            {
                toLoad.Add(new Chunk(loadedData, x * Chunk.chunkSize, y * Chunk.chunkSize));
                x++;
                if (x >= World.width) { x = 0; y++; }
            }
            
            List<TileObject> importedTiles = new List<TileObject>();
            for (int i = 0; i < loadedFile["tilePalette"].Count; i++)
            {
                importedTiles.Add(new TileObject( (int)loadedFile["tilePalette"][i]["sprId"], loadedFile["tilePalette"][i]["sprName"].ToString(),
                            (bool)loadedFile["tilePalette"][i]["collider"], (bool)loadedFile["tilePalette"][i]["transparent"], (bool)loadedFile["tilePalette"][i]["lightSrc"]));
            }
            TileAtlas.SetTileObjectArrayToAtlas(importedTiles.ToArray());
            
            List<TileObject> importedDungeonTiles = new List<TileObject>();
            for (int i = 0; i < loadedFile["dungeonTilePalette"].Count; i++)
            {
                importedDungeonTiles.Add(new TileObject( (int)loadedFile["dungeonTilePalette"][i]["sprId"], loadedFile["dungeonTilePalette"][i]["sprName"].ToString(),
                            (bool)loadedFile["dungeonTilePalette"][i]["collider"], (bool)loadedFile["dungeonTilePalette"][i]["transparent"], (bool)loadedFile["dungeonTilePalette"][i]["lightSrc"]));
            }
            TileAtlas.SetTileObjectArrayToDungeonAtlas(importedDungeonTiles.ToArray());

            mapObject.loadedWorld.CreateWorldWithExistingData(toLoad, loadedFile["mapName"].ToString(), (int)loadedFile["mapWidth"], (int)loadedFile["mapHeight"]);
            mapObject.initialized = true;
            return true;
        }

        public IEnumerator SaveMap()
        {
            MapViewport mapObject = FindObjectOfType<MapViewport>();
            Map toExport = new Map();

            toExport.mapName = World.worldName;
            toExport.author = "Momonyaro";
            toExport.mapWidth = World.width;
            toExport.mapHeight = World.height;
            toExport.playerStartPosX = mapObject.centerPosOnMap.x;
            toExport.playerStartPosY = mapObject.centerPosOnMap.y;
            toExport.spriteSize = 32;
            toExport.dungeonSpriteSize = 64;
            toExport.chnkPaths = chnkExporter.SaveChunks(campaignPath + toExport.mapName, mapObject.loadedWorld.worldData); // Campaigns/[MAPNAME]/mapname.map
            List<TilePaletteObj> toShrink = new List<TilePaletteObj>();
            foreach (var tileAtlasObj in TileAtlas.TileObjects)
            {
                toShrink.Add(new TilePaletteObj(tileAtlasObj.tile.sprite.name, tileAtlasObj.id, tileAtlasObj.collider, tileAtlasObj.transparent, tileAtlasObj.lightSource));
            }
            toExport.tilePalette = toShrink.ToArray();
            
            toShrink.Clear();
            foreach (var tileAtlasObj in TileAtlas.DngTileObjects)
            {
                toShrink.Add(new TilePaletteObj(tileAtlasObj.tile.sprite.name, tileAtlasObj.id, tileAtlasObj.collider, tileAtlasObj.transparent, tileAtlasObj.lightSource));
            }
            toExport.dungeonTilePalette = toShrink.ToArray();

            JsonData mapData = JsonUtility.ToJson(toExport);
            Directory.CreateDirectory(campaignPath + toExport.mapName + "/");
            string finalPath = campaignPath + toExport.mapName + "/" + toExport.mapName + mapExtention;
            itemExporter.SaveItemDatabase(campaignPath + toExport.mapName + "/");
            File.WriteAllText(finalPath, mapData.ToString());

            yield break;
        }

    }

    public struct Map
    {
        public string mapName;
        public string author;
        public int mapWidth;
        public int mapHeight;
        public int spriteSize;
        public int dungeonSpriteSize;
        public int playerStartPosX;
        public int playerStartPosY;
        public string[] chnkPaths;
        public TilePaletteObj[] tilePalette;
        public TilePaletteObj[] dungeonTilePalette;
    }


    [System.Serializable]
    public struct TilePaletteObj
    {
        public string sprName;
        public int sprId;
        public bool collider;
        public bool transparent;
        public bool lightSrc;

        public TilePaletteObj(string sprName, int sprId, bool collider, bool transparent, bool lightSrc)
        {
            this.sprName = sprName;
            this.sprId = sprId;
            this.collider = collider;
            this.transparent = transparent;
            this.lightSrc = lightSrc;
        }
    }
}