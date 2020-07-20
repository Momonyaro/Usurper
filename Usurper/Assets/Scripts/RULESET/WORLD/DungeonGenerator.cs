using System.Collections.Generic;
using UnityEngine;

namespace RULESET.WORLD
{
    public class DungeonGenerator
    {
        public const short MAX_CELLS = 64;    // This is the width or height, not the area!
        private const short CELL_SIZE = 2;    // This is the width or height, not the area!
        private const short RAND_ROOM_ATTEMTS = 30;
        private const short ROOM_PLACE_ATTEMPTS = 10;
        private const short DOOR_PLACE_ATTEMPTS = 300;
        private const short MAX_BACKTRACKS = 30*25 - 50;
        
        public DungeonCell[,] Cells;
        public int widthInCells  = 0;
        public int heightInCells = 0;

        public int[,] GenerateDungeon(int cellWidth, int cellHeight)
        {
            widthInCells = cellWidth;
            heightInCells = cellHeight;
            List<Stack<Vector2Int>> threads = new List<Stack<Vector2Int>>();

            //Create the cells!
            CreateCellGrid(cellWidth, cellHeight);
            //Place random rooms (run decor and item pass for each room instead of it's own thing)
            PlaceRooms(cellWidth, cellHeight, 4, 7, 2);
            //Create the maze of corridors!
            threads = GenerateMaze(cellWidth, cellHeight, 1);
            //Place doors
            PlaceDoors(cellWidth, cellHeight, 1);
            //Place entities

            threads.Reverse();
            BacktraceThreads(threads);

            PlaceDoors(cellWidth, cellHeight, 3);

            //At the end we return the completed map!
            return ConvertCellsToIntData(cellWidth, cellHeight);
        }

        private void CreateCellGrid(int cellWidth, int cellHeight)
        {
            Cells = new DungeonCell[cellWidth, cellHeight];
            for (int y = 0; y < cellHeight; y++)
                for (int x = 0; x < cellWidth; x++)
                {
                    Cells[x, y].Connected = false;

                    //This is to differentiate the tile

                    Cells[x, y].Tiles = new DungeonTile[4];
                    Cells[x, y].Tiles[0] = new DungeonTile() { TileType = DungeonTile.TileTypes.Wall, SpriteIndex = 0 };
                    Cells[x, y].Tiles[1] = new DungeonTile() { TileType = DungeonTile.TileTypes.Wall, SpriteIndex = 0 };
                    Cells[x, y].Tiles[2] = new DungeonTile() { TileType = DungeonTile.TileTypes.Wall, SpriteIndex = 0 };
                    Cells[x, y].Tiles[3] = new DungeonTile() { TileType = DungeonTile.TileTypes.Wall, SpriteIndex = 0 };
                }
        }

        private void PlaceRooms(int cellWidth, int cellHeight, int minRoomSize, int maxRoomSize, int minBorderDist)
        {
            //First choose the room size, then check random positions if that spot is available
            //If the check fails x times then try a new random room
            for (int q = 0; q < RAND_ROOM_ATTEMTS; q++)
            {
                int roomWidth = Random.Range(minRoomSize, maxRoomSize);
                int roomHeight = Random.Range(minRoomSize, maxRoomSize);

                for (int p = 0; p < ROOM_PLACE_ATTEMPTS; p++)
                {
                    //Pick a position within the map bounds.
                    //Check for cells with connected = true.
                    //If it's ok to place the room, do so by modifying the cells,
                    //setting them to connected and then breaking!

                    Vector2Int posToTest = new Vector2Int(Random.Range(minBorderDist, (cellWidth - minBorderDist) - roomWidth), 
                                                          Random.Range(minBorderDist, (cellHeight - minBorderDist) - roomWidth));

                    //Debug.Log("pos: " + posToTest + " size: " + roomWidth + ", " + roomHeight);

                    if (posToTest.x < minBorderDist || (posToTest.x + roomWidth ) > (cellWidth  - minBorderDist)) continue;
                    if (posToTest.y < minBorderDist || (posToTest.y + roomHeight) > (cellHeight - minBorderDist)) continue;

                    for (int y = 0; y < roomHeight; y++)
                        for (int x = 0; x < roomWidth; x++)
                        {
                            if (Cells[posToTest.x + x, posToTest.y + y].Connected) goto EndOfRandPlaceLoop;
                        }

                    //Draw the room!
                    for (int y = 0; y < roomHeight; y++)
                        for (int x = 0; x < roomWidth; x++)
                        {
                            Cells[posToTest.x + x, posToTest.y + y].Connected = true;
                            Cells[posToTest.x + x, posToTest.y + y].RoomCell = true;

                            Cells[posToTest.x + x, posToTest.y + y].Tiles[0].TileType = DungeonTile.TileTypes.Floor;
                            Cells[posToTest.x + x, posToTest.y + y].Tiles[0].SpriteIndex = 1;
                            if (x != roomWidth - 1)
                            {
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[1].TileType = DungeonTile.TileTypes.Floor;
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[1].SpriteIndex = 1;
                            }
                            if (y != roomHeight - 1)
                            {
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[2].TileType = DungeonTile.TileTypes.Floor;
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[2].SpriteIndex = 1;
                            }
                            if (x != roomWidth - 1 && y != roomHeight - 1)
                            {
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[3].TileType = DungeonTile.TileTypes.Floor;
                                Cells[posToTest.x + x, posToTest.y + y].Tiles[3].SpriteIndex = 1;
                            }
                        }

                    //Here we do a pass to decorate the room. We need to create a decoration pool for this!

                    EndOfRandPlaceLoop:
                    continue;
                }
            }

        }

        private List<Stack<Vector2Int>> GenerateMaze(int cellWidth, int cellHeight, int minBorderDist)
        {
            //We need to find a start position for the maze, start a maze thread, save it's position history and repeat until no unused positions are found.
            //Once this is done we can proceed to the door stage.
            bool continueMaze = true;
            List<Stack<Vector2Int>> mazeHistory = new List<Stack<Vector2Int>>();
            while (continueMaze)
            {
                for (int y = minBorderDist; y < cellHeight - minBorderDist; y++)
                {
                    for (int x = minBorderDist; x < cellWidth - minBorderDist; x++)
                    {
                        if (!Cells[x, y].Connected)
                        {
                            mazeHistory.Add(StartMazeThread(new Vector2Int(x, y), cellWidth, cellHeight, minBorderDist));
                            goto EndOfMazeWhile;
                        }
                    }
                }

                continueMaze = false;

            EndOfMazeWhile:
                continue;
            }
            return mazeHistory;
        }

        private Stack<Vector2Int> StartMazeThread(Vector2Int startPos, int cellWidth, int cellHeight, int minBorderDist)
        {
            bool continueThread = true;
            Stack<Vector2Int> threadHistory = new Stack<Vector2Int>();

            if (Cells[startPos.x - 1, startPos.y].Connected && !Cells[startPos.x - 1, startPos.y].RoomCell)
            {
                Cells[startPos.x - 1, startPos.y].Tiles[1].SpriteIndex = 1;
                Cells[startPos.x - 1, startPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x + 1, startPos.y].Connected && !Cells[startPos.x + 1, startPos.y].RoomCell)
            {
                Cells[startPos.x, startPos.y].Tiles[1].SpriteIndex = 1;
                Cells[startPos.x, startPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x, startPos.y - 1].Connected && !Cells[startPos.x, startPos.y - 1].RoomCell)
            {
                Cells[startPos.x, startPos.y - 1].Tiles[2].SpriteIndex = 1;
                Cells[startPos.x, startPos.y - 1].Tiles[2].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x, startPos.y + 1].Connected && !Cells[startPos.x, startPos.y + 1].RoomCell)
            {
                Cells[startPos.x, startPos.y].Tiles[2].SpriteIndex = 1;
                Cells[startPos.x, startPos.y].Tiles[2].TileType = DungeonTile.TileTypes.Floor;
            }

            while (continueThread)
            {
                threadHistory.Push(startPos);    //This is the currentPos in the thread.
                Cells[startPos.x, startPos.y].Connected = true;
                Cells[startPos.x, startPos.y].Tiles[0].TileType = DungeonTile.TileTypes.Floor;
                Cells[startPos.x, startPos.y].Tiles[0].SpriteIndex = 1;

                //Check cell neighbors for non-connected cells
                bool[] cellNeighbors = new bool[4] //1 means we can go there.
                {
                    (startPos.x > minBorderDist && !Cells[startPos.x - 1, startPos.y].Connected),
                    (startPos.x < (cellWidth - minBorderDist - 1) && !Cells[startPos.x + 1, startPos.y].Connected),
                    (startPos.y > minBorderDist && !Cells[startPos.x, startPos.y - 1].Connected),
                    (startPos.y < (cellHeight - minBorderDist - 1) && !Cells[startPos.x, startPos.y + 1].Connected)
                };

                if (!cellNeighbors[0] && !cellNeighbors[1] && !cellNeighbors[2] && !cellNeighbors[3]) break;

                Vector2Int oldPos = startPos;

                //Randomly pick a new cell to visit!
                while (true)
                {
                    if (cellNeighbors[0] && Random.value > 0.8) { startPos = new Vector2Int(startPos.x - 1, startPos.y); break; }
                    if (cellNeighbors[1] && Random.value > 0.8) { startPos = new Vector2Int(startPos.x + 1, startPos.y); break; }
                    if (cellNeighbors[2] && Random.value > 0.8) { startPos = new Vector2Int(startPos.x, startPos.y - 1); break; }
                    if (cellNeighbors[3] && Random.value > 0.8) { startPos = new Vector2Int(startPos.x, startPos.y + 1); break; }
                }

                //Draw the old cell and connect it and the new one!
                Vector2Int posDelta = oldPos - startPos;

                //Debug.Log(posDelta);

                // [-1, 0] = right, [1, 0] = left, [-1, 0] = up, [1, 0] = down

                if (posDelta.x < 0) { Cells[oldPos.x, oldPos.y].Tiles[1].SpriteIndex = 1; Cells[oldPos.x, oldPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor; }
                else if (posDelta.x > 0) { Cells[startPos.x, startPos.y].Tiles[1].SpriteIndex = 1; Cells[startPos.x, startPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor; }
                else if (posDelta.y < 0) { Cells[oldPos.x, oldPos.y].Tiles[2].SpriteIndex = 1; Cells[oldPos.x, oldPos.y].Tiles[2].TileType = DungeonTile.TileTypes.Floor; }
                else if (posDelta.y > 0) { Cells[startPos.x, startPos.y].Tiles[2].SpriteIndex = 1; Cells[startPos.x, startPos.y].Tiles[2].TileType = DungeonTile.TileTypes.Floor; }

            }

            if (Cells[startPos.x - 1, startPos.y].Connected && !Cells[startPos.x - 1, startPos.y].RoomCell)
            {
                Cells[startPos.x - 1, startPos.y].Tiles[1].SpriteIndex = 1;
                Cells[startPos.x - 1, startPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x + 1, startPos.y].Connected && !Cells[startPos.x + 1, startPos.y].RoomCell)
            {
                Cells[startPos.x, startPos.y].Tiles[1].SpriteIndex = 1;
                Cells[startPos.x, startPos.y].Tiles[1].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x, startPos.y - 1].Connected && !Cells[startPos.x, startPos.y - 1].RoomCell)
            {
                Cells[startPos.x, startPos.y - 1].Tiles[2].SpriteIndex = 1;
                Cells[startPos.x, startPos.y - 1].Tiles[2].TileType = DungeonTile.TileTypes.Floor;
            }
            else if (Cells[startPos.x, startPos.y + 1].Connected && !Cells[startPos.x, startPos.y + 1].RoomCell)
            {
                Cells[startPos.x, startPos.y].Tiles[2].SpriteIndex = 1;
                Cells[startPos.x, startPos.y].Tiles[2].TileType = DungeonTile.TileTypes.Floor;
            }

            return threadHistory;
        }

        private void PlaceDoors(int cellWidth, int cellHeight, int minBorderDist)
        {
            //We need a way to get a cell with a floor tile that's next to another cell with a floor tile
            //We then need to see if there is a wall between them...

            for (int i = 0; i < DOOR_PLACE_ATTEMPTS; i++)
            {
                Vector2Int posToTest = new Vector2Int(Random.Range(minBorderDist, (cellWidth - minBorderDist)),
                                                      Random.Range(minBorderDist, (cellHeight - minBorderDist)));

                while (!Cells[posToTest.x - 1, posToTest.y].RoomCell && Cells[posToTest.x - 1, posToTest.y].RoomCell && Cells[posToTest.x + 1, posToTest.y].RoomCell &&
                    Cells[posToTest.x, posToTest.y - 1].RoomCell && Cells[posToTest.x, posToTest.y + 1].RoomCell)
                {
                    posToTest = new Vector2Int(Random.Range(minBorderDist, (cellWidth - minBorderDist)),
                                               Random.Range(minBorderDist, (cellHeight - minBorderDist)));
                }

                if (Cells[posToTest.x, posToTest.y].RoomCell)
                {
                    //Check neighbors!
                    bool[] cellNeighbors = new bool[4] //1 means we can go there.
                    {
                        Cells[posToTest.x - 1, posToTest.y].Tiles[1].TileType == DungeonTile.TileTypes.Wall && Cells[posToTest.x - 1, posToTest.y].Connected,
                        Cells[posToTest.x, posToTest.y].Tiles[1].TileType == DungeonTile.TileTypes.Wall && Cells[posToTest.x + 1, posToTest.y].Connected,
                        Cells[posToTest.x, posToTest.y - 1].Tiles[2].TileType == DungeonTile.TileTypes.Wall && Cells[posToTest.x, posToTest.y - 1].Connected,
                        Cells[posToTest.x, posToTest.y].Tiles[2].TileType == DungeonTile.TileTypes.Wall && Cells[posToTest.x, posToTest.y + 1].Connected
                    };

                    if (!cellNeighbors[0] && !cellNeighbors[1] && !cellNeighbors[2] && !cellNeighbors[3]) { /*Debug.Log("All neighbors are set to false! No door placed at: " + posToTest);*/ continue; } 

                    //Randomly pick a cell!
                    while (true)
                    {
                        if (cellNeighbors[0] && Random.value > 0.8) //left
                        {
                            int yMin = (posToTest.y - 5 > 0) ? posToTest.y - 5 : 0;
                            int yMax = (posToTest.y + 5 < cellHeight) ? posToTest.y + 5 : cellHeight;
                            for (int y = yMin; y < yMax; y++)
                                if (Cells[posToTest.x - 1, y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[posToTest.x - 1, y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            int xMin = (posToTest.x - 5 > 0) ? posToTest.x - 5 : 0;
                            int xMax = (posToTest.x + 5 < cellWidth) ? posToTest.x + 5 : cellWidth;
                            for (int x = xMin; x < xMax; x++)
                                if (Cells[x, posToTest.y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[x, posToTest.y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            Cells[posToTest.x - 1, posToTest.y].Tiles[1].TileType = DungeonTile.TileTypes.Door;
                            Cells[posToTest.x - 1, posToTest.y].Tiles[1].SpriteIndex = 2;
                            //Debug.Log("We put a door at:" + (posToTest.x - 1) + "," + (posToTest.y));
                            break;
                        }
                        else if (cellNeighbors[1] && Random.value > 0.8) //right
                        {
                            int yMin = (posToTest.y - 5 > 0) ? posToTest.y - 5 : 0;
                            int yMax = (posToTest.y + 5 < cellHeight) ? posToTest.y + 5 : cellHeight;
                            for (int y = yMin; y < yMax; y++)
                                if (Cells[posToTest.x, y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[posToTest.x, y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            int xMin = (posToTest.x - 5 > 0) ? posToTest.x - 5 : 0;
                            int xMax = (posToTest.x + 5 < cellWidth) ? posToTest.x + 5 : cellWidth;
                            for (int x = xMin; x < xMax; x++)
                                if (Cells[x, posToTest.y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[x, posToTest.y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            Cells[posToTest.x, posToTest.y].Tiles[1].TileType = DungeonTile.TileTypes.Door;
                            Cells[posToTest.x, posToTest.y].Tiles[1].SpriteIndex = 2;
                            //Debug.Log("We put a door at:" + (posToTest.x) + "," + (posToTest.y));
                            break;
                        }
                        else if (cellNeighbors[2] && Random.value > 0.8) //up     (up is negative)
                        {
                            int yMin = (posToTest.y - 5 > 0) ? posToTest.y - 5 : 0;
                            int yMax = (posToTest.y + 5 < cellHeight) ? posToTest.y + 5 : cellHeight;
                            for (int y = yMin; y < yMax; y++)
                                if (Cells[posToTest.x, y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[posToTest.x, y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            int xMin = (posToTest.x - 5 > 0) ? posToTest.x - 5 : 0;
                            int xMax = (posToTest.x + 5 < cellWidth) ? posToTest.x + 5 : cellWidth;
                            for (int x = xMin; x < xMax; x++)
                                if (Cells[x, posToTest.y - 1].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[x, posToTest.y - 1].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            Cells[posToTest.x, posToTest.y - 1].Tiles[2].TileType = DungeonTile.TileTypes.Door;
                            Cells[posToTest.x, posToTest.y - 1].Tiles[2].SpriteIndex = 2;
                            //Debug.Log("We put a door at:" + (posToTest.x) + "," + (posToTest.y - 1));
                            break;
                        }
                        else if (cellNeighbors[3] && Random.value > 0.8) //down    (down is positive)
                        {
                            int yMin = (posToTest.y - 5 > 0) ? posToTest.y - 5 : 0;
                            int yMax = (posToTest.y + 5 < cellHeight) ? posToTest.y + 5 : cellHeight;
                            for (int y = yMin; y < yMax; y++)
                                if (Cells[posToTest.x, y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[posToTest.x, y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            int xMin = (posToTest.x - 5 > 0) ? posToTest.x - 5 : 0;
                            int xMax = (posToTest.x + 5 < cellWidth) ? posToTest.x + 5 : cellWidth;
                            for (int x = xMin; x < xMax; x++)
                                if (Cells[x, posToTest.y].Tiles[1].TileType == DungeonTile.TileTypes.Door || Cells[x, posToTest.y].Tiles[2].TileType == DungeonTile.TileTypes.Door) goto EndOfDoorLoop;

                            Cells[posToTest.x, posToTest.y].Tiles[2].TileType = DungeonTile.TileTypes.Door;
                            Cells[posToTest.x, posToTest.y].Tiles[2].SpriteIndex = 2;
                            //Debug.Log("We put a door at:" + (posToTest.x) + "," + (posToTest.y + 1));
                            break;
                        }
                    }
                }

            EndOfDoorLoop:
                continue;
            }

        }

        private void BacktraceThreads(List<Stack<Vector2Int>> threads)
        {
            int backtracks = 0;
            for (int q = 0; q < threads.Count; q++)
            {
                Stack<Vector2Int> currentStack = threads[q];

                bool continueThread = true;
                
                while (currentStack.Count > 0 && continueThread)
                {
                    //check neighbors for a door. If none exist, pop and go to the next element
                    Vector2Int pos = currentStack.Pop();

                    //Check neighbors!
                    bool[] cellNeighbors = new bool[4] //1 means we can go there.
                    {
                        Cells[pos.x - 1, pos.y].Tiles[1].TileType == DungeonTile.TileTypes.Door && currentStack.Count >= 3,
                        Cells[pos.x, pos.y].Tiles[1].TileType == DungeonTile.TileTypes.Door && currentStack.Count >= 3,
                        Cells[pos.x, pos.y - 1].Tiles[2].TileType == DungeonTile.TileTypes.Door && currentStack.Count >= 3,
                        Cells[pos.x, pos.y].Tiles[2].TileType == DungeonTile.TileTypes.Door && currentStack.Count >= 3
                    };

                    if (!cellNeighbors[0] && !cellNeighbors[1] && !cellNeighbors[2] && !cellNeighbors[3])
                    {
                        Cells[pos.x, pos.y].Connected = false;

                        Cells[pos.x, pos.y].Tiles[0].SpriteIndex = 0;
                        Cells[pos.x, pos.y].Tiles[0].TileType = DungeonTile.TileTypes.Wall;
                        Cells[pos.x, pos.y].Tiles[1].SpriteIndex = 0;
                        Cells[pos.x, pos.y].Tiles[1].TileType = DungeonTile.TileTypes.Wall;
                        Cells[pos.x, pos.y].Tiles[2].SpriteIndex = 0;
                        Cells[pos.x, pos.y].Tiles[2].TileType = DungeonTile.TileTypes.Wall;
                        Cells[pos.x, pos.y].Tiles[3].SpriteIndex = 0;
                        Cells[pos.x, pos.y].Tiles[3].TileType = DungeonTile.TileTypes.Wall;

                        Cells[pos.x - 1, pos.y].Tiles[1].SpriteIndex = 0;
                        Cells[pos.x - 1, pos.y].Tiles[1].TileType = DungeonTile.TileTypes.Wall;
                        Cells[pos.x, pos.y - 1].Tiles[2].SpriteIndex = 0;
                        Cells[pos.x, pos.y - 1].Tiles[2].TileType = DungeonTile.TileTypes.Wall;

                        backtracks++;
                        if (backtracks >= MAX_BACKTRACKS) continueThread = false;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private int[,] ConvertCellsToIntData(int cellWidth, int cellHeight)
        {
            int[,] intData = new int[cellWidth * CELL_SIZE, cellHeight * CELL_SIZE];
            for (int y = 0; y < cellHeight; y++)
            {
                for (int x = 0; x < cellWidth; x++)
                {
                    intData[(x * CELL_SIZE) + 0, (y * CELL_SIZE) + 0] = Cells[x, y].Tiles[0].SpriteIndex;
                    intData[(x * CELL_SIZE) + 1, (y * CELL_SIZE) + 0] = Cells[x, y].Tiles[1].SpriteIndex;
                    intData[(x * CELL_SIZE) + 0, (y * CELL_SIZE) + 1] = Cells[x, y].Tiles[2].SpriteIndex;
                    intData[(x * CELL_SIZE) + 1, (y * CELL_SIZE) + 1] = Cells[x, y].Tiles[3].SpriteIndex;
                }
            }

            return intData;
        }
        
    }

    public struct DungeonCell
    {
        public DungeonTile[] Tiles;
        public bool Connected;
        public bool RoomCell;
    }

    public struct DungeonTile
    {
        public enum TileTypes
        {
            Floor,
            Wall,
            Door,
        }
        
        public int SpriteIndex; //We use the tileEditor to get the wanted sprite!
        public TileTypes TileType; //Used to decide rendering mode of the blocks!

        public DungeonTile(bool ignore = false)
        {
            SpriteIndex = 0;
            TileType = TileTypes.Wall;
        }
    }
}