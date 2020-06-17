using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk
{
    public const short chunkSize = 64;
    public int[,] mapData;
    private int xStartPos;
    private int yStartPos;


    public Chunk(int[,] mapData, int xStart, int yStart)
    {
        if (mapData == null) { SetChunkMapData(GetFallbackData()); return; }
        SetChunkMapData(mapData);
    }

    public Chunk(int xStart, int yStart)
    {
        xStartPos = xStart;
        yStartPos = yStart;
        SetChunkMapData(GetFallbackData());
    }


    public void SetChunkMapData(int[,] mapData)
    {
        this.mapData = mapData;
    }

    public int[,] GetChunkMapData()
    {
        if (mapData == null) return GetFallbackData();
        return mapData;
    }

    private int[,] GetFallbackData()
    {
        int[,] fallback = new int[chunkSize, chunkSize];
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                fallback[x, y] = 1;
            }
        }
        fallback[0            , 0] = 3; fallback[0            , chunkSize - 1] = 3;
        fallback[chunkSize - 1, 0] = 3; fallback[chunkSize - 1, chunkSize - 1] = 3;
        fallback[5, 5] = 2;
        return fallback;
    }

    public Vector2Int GetChunkStartPos()
    {
        return new Vector2Int(xStartPos, yStartPos);
    }

}
