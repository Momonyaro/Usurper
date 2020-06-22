using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RENDERER.MAP;
using LitJson;

namespace EDITOR.EXPORT
{

    public class ChunkExporter
    {
        private const string chunkFolderName = "/Regions";
        private const string chunkExtention = ".chnk";

        public List<int[,]> LoadChunks(List<string> chunkPaths, string mapName)
        {
            List<int[,]> toReturn = new List<int[,]>();
            Debug.Log("looking for " + chunkPaths.Count + " chnk files...");
            foreach (var chunkPath in chunkPaths)
            {
                JsonData fileData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/Campaigns/" + mapName + "/" + chunkPath));
                int chunkSize = (int)fileData["chunkSize"];
                Debug.Log("found chunk with chunkSize: " + chunkSize);
                int[,] chunkData = new int[chunkSize, chunkSize]; 
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int newData = (int)fileData["chunkData"][x + y * chunkSize];
                        chunkData[x, y] = newData;
                    }
                }
                toReturn.Add(chunkData);
            }
            Debug.Log("returning loaded chunkData with size of: " + toReturn.Count);
            return toReturn;
        }

        public string[] SaveChunks(string mapPath, List<Chunk> chunks)
        {
            List<string> chunkPaths = new List<string>();

            //For each chunk passed, generate a .chnk file and add it's path to chunkPaths.
            for (int i = 0; i < chunks.Count; i++)
            {
                ExportChunk toExport = new ExportChunk(Chunk.chunkSize, chunks[i].mapData);
                JsonData chunkData = JsonUtility.ToJson(toExport);
                if (!Directory.Exists(mapPath + chunkFolderName))
                {
                    Directory.CreateDirectory(mapPath + chunkFolderName);
                }
                string finalPath = mapPath + chunkFolderName + "/chnk_" + i + chunkExtention;
                File.WriteAllText(finalPath, chunkData.ToString());
                chunkPaths.Add(chunkFolderName + "/chnk_" + i + chunkExtention);
            }

            return chunkPaths.ToArray();
        }

    }

    public struct ExportChunk
    {
        public int chunkSize;
        public int[] chunkData;

        public ExportChunk(int chunkSize, int[,] intData)
        {
            this.chunkSize = chunkSize;
            this.chunkData = new int[intData.Length];
            for (int y = 0; y < Chunk.chunkSize; y++)
            {
                for (int x = 0; x < Chunk.chunkSize; x++)
                {
                    this.chunkData[x + y * Chunk.chunkSize] = intData[x, y];
                }
            }
        }
    }
}