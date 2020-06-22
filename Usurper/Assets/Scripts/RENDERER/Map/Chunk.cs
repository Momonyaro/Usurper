using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RENDERER.MAP
{

    [System.Serializable]
    public class Chunk
    {
        public const short chunkSize = 64;
        public int[,] mapData;
        private int xStartPos;
        private int yStartPos;


        public Chunk(int[,] mapData, int xStart, int yStart)
        {
            xStartPos = xStart;
            yStartPos = yStart;
            if (mapData == null) { Debug.Log("Recived null mapdata, using fallback"); SetChunkMapData(GetFallbackData()); return; }
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
            return fallback;
        }

        public Vector2Int GetChunkStartPos()
        {
            return new Vector2Int(xStartPos, yStartPos);
        }

        public void SetChunkStartPos(int x, int y)
        {
            xStartPos = x;
            yStartPos = y;
        }

    }

}