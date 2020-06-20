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
            return new int[chunkSize, chunkSize];
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