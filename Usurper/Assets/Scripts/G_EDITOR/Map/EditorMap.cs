using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RENDERER.UTILS.Atlas;
using RENDERER.UTILS;
using RENDERER.MAP;
using UnityEngine.UI;

namespace EDITOR.MAP
{
    public enum MAP_DISPLAY_MODES
    {
        DEFAULT,    //Draw the imported data as usual
        COLLIDER,   //Here we draw the tiles with colliders as a Cross and the others as void
        LIGHTS,
    }

    public enum MAP_BOUNDS_MODIFY_DIRECTIONS
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public class EditorMap : MonoBehaviour
    {
        int width = 2;
        int height = 2;

        public GameObject editorChunkPrefab;

        public List<Chunk> mapData = new List<Chunk>();
        public static MAP_DISPLAY_MODES MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.DEFAULT;
        public Text boundsInfoText;


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

            WipeCurrentTilemaps();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Chunk cData = new Chunk(x * Chunk.chunkSize, y * Chunk.chunkSize);
                    mapData.Add(cData);
                    GameObject newChunkGameObject = Instantiate(editorChunkPrefab, new Vector3(x * Chunk.chunkSize, y * Chunk.chunkSize, 0), Quaternion.identity, transform);
                    newChunkGameObject.GetComponent<EditorChunk>().SetChunkData(cData);
                }
            }
            CreateBoundsInfo();
        }

        public void CreateCurrentMap()
        {
            WipeCurrentTilemaps();
            foreach (var chunk in mapData)
            {
                Vector3 cPos = new Vector3(chunk.GetChunkStartPos().x, chunk.GetChunkStartPos().y);
                GameObject newChunkGameObject = Instantiate(editorChunkPrefab, cPos, Quaternion.identity, transform);
                newChunkGameObject.GetComponent<EditorChunk>().SetChunkData(chunk);
            }     
            CreateBoundsInfo();
        }

        public void DrawAndAddNewChunks(List<Chunk> newChunks)
        {
            foreach (var chunk in newChunks)
            {
                Vector3 cPos = new Vector3(chunk.GetChunkStartPos().x, chunk.GetChunkStartPos().y);
                GameObject newChunkGameObject;
                //Gonna look real wierd until we get a proper view cull method
                //if (Vector3.Distance(cPos + new Vector3(Chunk.chunkSize/2, Chunk.chunkSize/2), Camera.main.transform.position) < 128)
                //{
                    newChunkGameObject = Instantiate(editorChunkPrefab, cPos, Quaternion.identity, transform);
                    newChunkGameObject.GetComponent<EditorChunk>().SetChunkData(chunk);
                //}

                mapData.Add(chunk);
            }
            CreateBoundsInfo();
        }

        public void WipeCurrentTilemaps()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
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

        public void ResizeEditorMapBounds(int value, MAP_BOUNDS_MODIFY_DIRECTIONS direction)
        {
            Debug.Log(value + " " + direction);
            List<Chunk> newChunks = new List<Chunk>();
            //Resize map size
            switch (direction)
            {
                case MAP_BOUNDS_MODIFY_DIRECTIONS.UP: height += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN: height += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT: width += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.RIGHT: width += value; break;
            }

            //a positive down or left value means, shift all chunks up by a chunkSize and create new chunks at 0 x or y
            //a negative down or left means, remove chunks at 0 x or y and shift remaining chunks down by a chunkSize
            if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN || direction == MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT)
            {
                for (int i = 0; i < mapData.Count; i++)
                {
                    Vector2Int cStartPos = mapData[i].GetChunkStartPos();
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN) cStartPos += new Vector2Int(0, value * Chunk.chunkSize);
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT) cStartPos += new Vector2Int(value * Chunk.chunkSize, 0);
                    mapData[i].SetChunkStartPos(cStartPos.x, cStartPos.y);
                }

                // Remove chunks with positions less than 0
                if (value < 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            foreach (var chunk in mapData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.y < 0)
                                {
                                    Debug.Log("Deleting chunk at " + cStartPos + " Because it's Y value is less than 0");
                                    mapData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < height; y++)
                        {
                            foreach (var chunk in mapData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.x < 0)
                                {
                                    Debug.Log("Deleting chunk at " + cStartPos + " Because it's X value is less than 0");
                                    mapData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                }

                //Here we add the extra chunks to fill out the new bounds
                if (value > 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN)
                    {
                        for (int x = 0; x < width; x++) { Chunk cData = new Chunk(x * Chunk.chunkSize, 0); mapData.Add(cData); }
                    }
                    else 
                    {
                        for (int y = 0; y < height; y++) { Chunk cData = new Chunk(0, y * Chunk.chunkSize); mapData.Add(cData); }
                    }
                }
                CreateCurrentMap();
            }

            //a positive up or right value means, add a extra row or column of chunks for the desired direction
            //a negative up or right means, remove the row or column of chunks so that it correctly represents the width and height of the map.
            if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP || direction == MAP_BOUNDS_MODIFY_DIRECTIONS.RIGHT)
            {
                if (value < 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            foreach (var chunk in mapData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.y >= (height) * Chunk.chunkSize)
                                {
                                    Debug.Log("Deleting chunk at " + cStartPos + " Because it's Y value is less than " + (height) * Chunk.chunkSize);
                                    mapData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < height; y++)
                        {
                            foreach (var chunk in mapData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.x >= (width) * Chunk.chunkSize)
                                {
                                    Debug.Log("Deleting chunk at " + cStartPos + " Because it's X value is less than " + (width) * Chunk.chunkSize);
                                    mapData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (value > 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP)
                    {
                        for (int x = 0; x < width; x++) 
                        { 
                            Chunk cData = new Chunk(x * Chunk.chunkSize, (height - 1) * Chunk.chunkSize); 
                            newChunks.Add(cData);
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < height; y++) 
                        { 
                            Chunk cData = new Chunk((width - 1) * Chunk.chunkSize, y * Chunk.chunkSize); 
                            newChunks.Add(cData);
                        }
                    }
                }
                DrawAndAddNewChunks(newChunks);
            }
        }

        public void CreateBoundsInfo()
        {
            boundsInfoText.text = "'TEST_MAP'\nWidth = " + width + " Height = " + height;
        }

        public void OrderMapRedraw()
        {
            int chunks = transform.childCount;
            for (int i = 0; i < chunks; i++)
            {
                transform.GetChild(i).GetComponent<EditorChunk>().DrawChunk();
            }
        }
    }
}