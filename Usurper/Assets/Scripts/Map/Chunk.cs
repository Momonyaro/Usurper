﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public const short chunkSize = 64;
    private int[,] mapData;
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
        return fallback;
    }

    public Vector2Int GetChunkStartPos()
    {
        return new Vector2Int(xStartPos, yStartPos);
    }

}