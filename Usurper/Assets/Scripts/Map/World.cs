using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    private const int renderDiameter = 3;
    public float arbitraryChunkDistance = 72;

    // We need to store chunks so that it looks like this, where P is the chunk where the player currently is.
    //  0 0 0
    //  0 P 0
    //  0 0 0
    //Edge cases can be dangerous, perhaps if no chunk is there, we return a nullChunk where all tileData = -1 (Invalid Tile)
    //We load the chunks by holding the chunk world position range ex. 0-128 and then if we have a SetPlayerPosition(Vector2 pos)
    //from this pos vector we can see what chunk the player is on and if we need to load new ones.

    public Chunk[] worldData = new Chunk[renderDiameter * renderDiameter];

    //When we have the data, we need to export 31x31 tiles to a tilemap where the player is at tile 31x31 (the center). This could mean that chunk
    //edge cases could be bad but if we check the x,y axis for chunk x, y = 0 and x, y = chunksize and load from the next chunk if true

    public void Init()
    {
        for (int y = 0; y < renderDiameter; y++)
        {
            for (int x = 0; x < renderDiameter; x++)
            {
                int xOffset = x * Chunk.chunkSize;
                int yOffset = y * Chunk.chunkSize;
                worldData[x + y * renderDiameter] = new Chunk(xOffset, yOffset);
                Debug.Log("World added chunk att start positions" + new Vector2Int(xOffset, yOffset));
            }
        }
    }

    public int[,] GetWorldDataAtPoint(Vector2Int pointOnWorld)
    {
        //first check if pointOnWorld is on the loaded world! If not, we will have to fetch a chunk on that position from .map files later
        //How do we load the missing data from other chunks?
        int[,] viewportData = new int[31, 31]; //x, y range = 0-30

        for (int i = 0; i < worldData.Length; i++)
        {
                //We need to check if this chunk contain data within the renderDistance
                //Since we have the world positions of the chunks and the player, let's
                //compare those and see if we get a hit.
                if (GetDistanceBetweenChunkAndPoint(worldData[i], pointOnWorld) < arbitraryChunkDistance)
                {
                    //Read Chunk Data and write it to the viewportData buffer!
                    viewportData = WriteChunkDataToViewport(viewportData, worldData[i], pointOnWorld);
                }
        }

        return viewportData;
    }

    private int[,] WriteChunkDataToViewport(int[,] viewportData, Chunk chunk, Vector2Int pointOnWorld)
    {
        //based on the pointOnWorld +- viewportRadius we can fetch the tiles we want and write them to the buffer.
        Debug.Log("Writing data from chunk at " + chunk.GetChunkStartPos() + " to viewport");
        int[,] chunkMapData = chunk.GetChunkMapData();
        int halfSize = Mathf.FloorToInt(MapViewport.viewPortRadius / 2);
        Rect renderArea = new Rect(pointOnWorld.x - halfSize, pointOnWorld.y - halfSize, 
                                    MapViewport.viewPortRadius, MapViewport.viewPortRadius);
        Debug.Log("RenderArea: " + renderArea);

        for (int y = 0; y < Chunk.chunkSize; y++)
        {
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                Vector2Int posToTest = chunk.GetChunkStartPos() + new Vector2Int(x, y);
                if (renderArea.Contains(posToTest))
                {
                    int viewX = posToTest.x - Mathf.RoundToInt(renderArea.xMin); 
                    int viewY = posToTest.y - Mathf.RoundToInt(renderArea.yMin);
                    viewportData[viewX, viewY] = chunkMapData[x, y];
                }
            }
        }

        return viewportData;
    }

    private float GetDistanceBetweenChunkAndPoint(Chunk chunk, Vector2Int pos)
    {
        //Get center of chunk
        if (chunk == null) { Debug.Log("Chunk was null? Please look into it!"); return 666;}
        Vector2 chunkCenter = new Vector2(Chunk.chunkSize / 2, Chunk.chunkSize/2);
        return Vector2.Distance(chunk.GetChunkStartPos() + chunkCenter, pos);
    }
}
