using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RULESET.MANAGERS;
using RULESET.ENTITIES;
using LitJson;


namespace EDITOR.EXPORT
{
    public class DungeonExporter
    {
        //This class is responsible for exporting the species and the actors.
        //Later we have to decide how we want to store actors when dialogue is a factor. (Perhaps we'll keep this as a mega file?)

        private const string fileExtention = ".dbase";

        public List<Gate> LoadDungeons(string mapName)
        {
            List<Gate> toReturn = new List<Gate>();
            JsonData fileData = JsonMapper.ToObject(File.ReadAllText(mapName + "/" + "Dungeons" + fileExtention));
            for (int i = 0; i < fileData["gates"].Count; i++)
            {
                int fetchedWidth = (int)fileData["gates"][i]["width"];
                int fetchedHeight = (int)fileData["gates"][i]["height"];
                int[,] fetchedDngData = new int[fetchedWidth, fetchedHeight];

                for (int y = 0; y < fetchedHeight; y++)
                {
                    for (int x = 0; x < fetchedWidth; x++)
                    {
                        fetchedDngData[x, y] = (int)fileData["gates"][i]["dngData"][x + y * fetchedWidth];
                    }
                }

                Gate a = new Gate()
                {
                    name = fileData["gates"][i]["name"].ToString(),
                    x = (int)fileData["gates"][i]["x"],
                    y = (int)fileData["gates"][i]["y"],
                    width = fetchedWidth,
                    height = fetchedHeight,
                    dngData = fetchedDngData
                };
                toReturn.Add(a);
            }
            return toReturn;
        }

        public void SaveDungeons(string mapPath)
        {
            string finalPath = mapPath + "/Dungeons" + fileExtention;
            DungeonBlock export = new DungeonBlock(GateManager.Gates);

            JsonData fileData = JsonMapper.ToJson(export);

            File.WriteAllText(finalPath, fileData.ToString());
        }
    }

    public struct DungeonBlock
    {
        public List<CompressedGate> gates;

        public DungeonBlock(List<Gate> bigGates)
        {
            gates = new List<CompressedGate>();
            for (int i = 0; i < bigGates.Count; i++)
            {
                gates.Add(new CompressedGate(bigGates[i]));
            }
        }
    }

    public struct CompressedGate
    {
        public string name;
        public int x;
        public int y;
        public int width;
        public int height;
        public int[] dngData;

        public CompressedGate(Gate toCompress)
        {
            name = toCompress.name;
            x = toCompress.x;
            y = toCompress.y;
            width = toCompress.width;
            height = toCompress.height;
            Debug.Log("width: " + width + ", height: " + height);
            dngData = new int[width * width];
            for (int q = 0; q < toCompress.height; q++)
            {
                for (int p = 0; p < toCompress.width; p++)
                {
                    dngData[p + q * toCompress.width] = toCompress.dngData[p, q];
                    if (toCompress.dngData[p, q] == 0) Debug.LogError("INVALID:DUNGEON:ID");
                }
            }
        }
    }
}