using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RENDERER.UTILS.Atlas;
using RENDERER.UTILS;
using RENDERER.MAP;

namespace EDITOR.MAP
{
    public enum MAP_DISPLAY_MODES
    {
        DEFAULT,    //Draw the imported data as usual
        COLLIDER,   //Here we draw the tiles with colliders as a Cross and the others as void
    }

    public class EditorMap : MonoBehaviour
    {
        int width = 2;
        int height = 2;

        public GameObject editorChunkPrefab;

        public List<Chunk> mapData = new List<Chunk>();
        public static MAP_DISPLAY_MODES MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.COLLIDER;


        private void Start()
        {
            StartCoroutine(LoadMapIfAtlasReady());
        }

        public IEnumerator LoadMapIfAtlasReady()
        {
            while (!StreamingResourceLoader.finishedReading) { yield return null; }

            //Here we load it
            CreateMap(width, height);

            yield break;
        }

        public void CreateMap(int w, int h)
        {
            this.width = w;
            this.height = h;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Chunk cData = new Chunk(x * Chunk.chunkSize, y * Chunk.chunkSize);
                    mapData.Add(cData);
                    GameObject newChunkGameObject = Instantiate(editorChunkPrefab, transform);
                    newChunkGameObject.GetComponent<EditorChunk>().SetChunkData(cData);
                }
            }
        }

        public Chunk OverwriteChunkDataOnMap(Chunk updatedChunk)
        {
            Debug.Log("Replacing mapdata at: " + updatedChunk.GetChunkStartPos());
            for (int i = 0; i < mapData.Count; i++)
            {
                Vector2Int mapDataPos = mapData[i].GetChunkStartPos();
                Vector2Int newDataPos = updatedChunk.GetChunkStartPos();
                if (mapDataPos.x == newDataPos.x && mapDataPos.y == newDataPos.y)
                {
                    //Gottem!
                    mapData[i] = updatedChunk;
                    return mapData[i];
                }
            }
            Debug.Log("FAILED TO FIND CHUNK IN MAPDATA AT: " + updatedChunk.GetChunkStartPos());
            return updatedChunk;
        }
    }
}