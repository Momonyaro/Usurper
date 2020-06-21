using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;
using LitJson;

namespace EDITOR.EXPORT
{

    public class MapExporter : MonoBehaviour
    {
        private string campaignPath = Application.streamingAssetsPath + "/Campaigns/";
        private const string mapExtention = ".map";

        ChunkExporter chnkExporter = new ChunkExporter();


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartCoroutine(SaveMap());
            }
        }

        public IEnumerator LoadMap(string mapName)
        {
            MapViewport mapObject = FindObjectOfType<MapViewport>();
            JsonData loadedFile = File.ReadAllText(campaignPath + mapName);

            World.worldName = loadedFile["mapName"].ToString();
            Debug.Log(loadedFile["author"].ToString());
            World.width = (int)loadedFile["mapWidth"];
            World.height = (int)loadedFile["mapHeight"];
            mapObject.playerPosOnMap = new Vector2Int((int)loadedFile["playerStartPosX"], (int)loadedFile["playerStartPosY"]);
            List<string> chunkPaths = new List<string>();
            for (int i = 0; i < loadedFile["chnkPaths"].Count; i++)
            {
                chunkPaths.Add(loadedFile["chnkPaths"][i].ToString());
            }

            List<Chunk> toLoad = new List<Chunk>();
            List<int[,]> loadedIntData = chnkExporter.LoadChunks(chunkPaths);
            int x = 0;
            int y = 0;
            foreach (var loadedData in loadedIntData)
            {
                toLoad.Add(new Chunk(loadedData, x * Chunk.chunkSize, y * Chunk.chunkSize));
                x++;
                if (x > World.width) { x = 0; y++; }
            }
            
            mapObject.loadedWorld.CreateWorldWithExistingData(toLoad);

            yield break;
        }

        public IEnumerator SaveMap()
        {
            MapViewport mapObject = FindObjectOfType<MapViewport>();
            Map toExport = new Map();

            toExport.mapName = World.worldName;
            toExport.author = "Momonyaro";
            toExport.mapWidth = World.width;
            toExport.mapHeight = World.height;
            toExport.playerStartPosX = mapObject.playerPosOnMap.x;
            toExport.playerStartPosY = mapObject.playerPosOnMap.y;
            toExport.chnkPaths = chnkExporter.SaveChunks(campaignPath + toExport.mapName, mapObject.loadedWorld.worldData); // Campaigns/[MAPNAME]/mapname.map
            List<TilePaletteObj> toShrink = new List<TilePaletteObj>();
            foreach (var tileAtlasObj in TileAtlas.tileObjects)
            {
                toShrink.Add(new TilePaletteObj(tileAtlasObj.tile.sprite.name, tileAtlasObj.id, tileAtlasObj.collider, tileAtlasObj.lightSource));
            }
            toExport.tilePalette = toShrink.ToArray();

            JsonData mapData = JsonUtility.ToJson(toExport);
            Directory.CreateDirectory(campaignPath + toExport.mapName + "/");
            string finalPath = campaignPath + toExport.mapName + "/" + toExport.mapName + mapExtention;
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
        public int playerStartPosX;
        public int playerStartPosY;
        public string[] chnkPaths;
        public TilePaletteObj[] tilePalette;
    }


    [System.Serializable]
    public struct TilePaletteObj
    {
        public string sprName;
        public int sprId;
        public bool collider;
        public bool lightSrc;

        public TilePaletteObj(string sprName, int sprId, bool collider, bool lightSrc)
        {
            this.sprName = sprName;
            this.sprId = sprId;
            this.collider = collider;
            this.lightSrc = lightSrc;
        }
    }
}