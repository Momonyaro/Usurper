using RENDERER.MAP;
using RENDERER.UTILS.Atlas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RULESET.MANAGERS
{
    public class GateManager : MonoBehaviour
    {
        public static List<Gate> Gates = new List<Gate>();

        //The gates will be interfaced with the entityRenderer when in the editor and will display the gate positions.
        //We will need simple add/remove functionality as well as the ability to edit the gate's properties.

        //When in the game, gates are invisible so you have to place a tile to represent it. (like an entrance!)
        //Entering this tile will prompt a pop-up asking if you want to enter. Pressing yes will switch to the dungeon view
        //  and load the dungeon. The dungeon view is a 3D view controlled with traditional WASD controls.

        public static List<Gate> GetRelevantGates(Vector2Int center, int range)
        {
            List<Gate> toReturn = new List<Gate>();
            for (int i = 0; i < Gates.Count; i++)
            {
                Vector2Int gatePos = new Vector2Int(Gates[i].x, Gates[i].y);
                //Debug.Log(gatePos);
                if (gatePos.x <= center.x + range && gatePos.x > center.x - range &&
                    gatePos.y <= center.y + range && gatePos.y > center.y - range)
                {
                    toReturn.Add(Gates[i]);
                }
            }
            Debug.Log(toReturn.Count);
            return toReturn;
        }

        public static bool PlaceAtPosition(int x, int y)
        {
            PointerImageGhost.placingEntityLayer = 0;
            for (int i = 0; i < Gates.Count; i++)
            {
                if (Gates[i].x == x && Gates[i].y == y)
                {
                    return false;
                }
            }
            GateManager.Gates.Add(new Gate("New Gate", x, y));
            return true;
        }

        public static bool RemoveAtPosition(int x, int y)
        {
            PointerImageGhost.placingEntityLayer = 0;
            for (int i = 0; i < Gates.Count; i++)
            {
                if (Gates[i].x == x && Gates[i].y == y)
                {
                    GateManager.Gates.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

    }

    public struct Gate
    {
        public string name;
        public int x;
        public int y;
        public int[,] dngData;

        public Gate(string name, int x, int y, int[,] dngData)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.dngData = dngData;
        }

        public Gate(string name, int x, int y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            dngData = new int[75,50];
        }
    }
}
