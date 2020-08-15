using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RULESET.WORLD
{
    public class DrunkardsAlgorithm : DungeonAlgorithm
    {
        private int minBorderDist = 2;
        private int maxEntrancesToConsider = 30;
        private int width;
        private int height;

        public override int[,] Generate(int width, int height)
        {
            this.width = width; this.height = height;
            CreateCanvas(width, height);

            DrunkardsWalk();
            PlaceEntrance();

            return ConvertToIntData(width, height);
        }

        private void DrunkardsWalk()
        {
            Vector2Int currentPosition = new Vector2Int(Random.Range(minBorderDist, width - minBorderDist - 1), Random.Range(minBorderDist, height - minBorderDist - 1));

            Tiles[currentPosition.x, currentPosition.y] = TILE_TYPES.FLOOR;
            int currentSteps = 1;
            int maxSteps = ((width - minBorderDist - 1) * (height - minBorderDist - 1)) / 2;

            while (currentSteps <= maxSteps)
            {
                //We need to take a random step and if we destroy a wall, we increment currentSteps.
                int dir = Random.Range(0, 4);

                switch(dir)
                {
                    case 0:     // LEFT
                        currentPosition += Vector2Int.left;
                        break;
                    case 1:     // RIGHT
                        currentPosition += Vector2Int.right;
                        break;
                    case 2:     // UP
                        currentPosition += Vector2Int.up;
                        break;
                    case 3:     // DOWN
                        currentPosition += Vector2Int.down;
                        break;
                }

                currentPosition.Clamp(new Vector2Int(minBorderDist, minBorderDist), new Vector2Int(width - minBorderDist - 1, height - minBorderDist - 1));

                if (Tiles[currentPosition.x, currentPosition.y] == TILE_TYPES.WALL)
                {
                    Tiles[currentPosition.x, currentPosition.y] = TILE_TYPES.FLOOR;
                    currentSteps++;
                }
            }
        }

        private void PlaceEntrance()
        {
            //Perhaps we find x number of possible entrances and check for the most walls < 8 next to that position. Pick the position with most walls.
            int currentEntrances = 0;
            maxEntrancesToConsider = (width * height) / 4;
            List<Vector3Int> doorPositions = new List<Vector3Int>();
            while (currentEntrances < maxEntrancesToConsider)
            {
                Vector2Int randomPos = new Vector2Int(Random.Range(minBorderDist, width - minBorderDist - 1), Random.Range(minBorderDist, height - minBorderDist - 1));

                if (Tiles[randomPos.x, randomPos.y] == TILE_TYPES.WALL)
                {
                    int neighborCount = 0;
                    for (int y = -1; y <= 1; y++)
                        for (int x = -1; x <= 1; x++)
                        {
                            if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

                            if (Tiles[randomPos.x + x, randomPos.y + y] == TILE_TYPES.WALL)
                            {
                                neighborCount++;
                            }
                        }
                    doorPositions.Add(new Vector3Int(randomPos.x, randomPos.y, neighborCount));
                    currentEntrances++;
                }
            }

            Vector3Int bestPosition = new Vector3Int(0, 0, 0);
            for (int q = 0; q < doorPositions.Count; q++)
            {
                if (doorPositions[q].z >= 4) continue;
                if (doorPositions[q].z >  bestPosition.z)
                {
                    bestPosition = doorPositions[q];
                }
            }
            Tiles[bestPosition.x, bestPosition.y] = TILE_TYPES.ENTRANCE;
        }
    }
}
