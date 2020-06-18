using UnityEngine;
using System.Collections.Generic;
using RENDERER.UTILS.Atlas;
using RENDERER.MAP;

namespace EDITOR.MAP
{
    public class EditorMap : MonoBehaviour
    {
        int width = 1;
        int height = 1;

        public GameObject editorChunkPrefab;

        public List<Chunk> mapData = new List<Chunk>();

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
            return updatedChunk;
        }
    }
}