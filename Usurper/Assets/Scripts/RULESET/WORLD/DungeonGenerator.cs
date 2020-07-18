using UnityEngine;

namespace RULESET.WORLD
{
    public class DungeonGenerator
    {
        public const short MAX_CELLS = 64;    // This is the width or height, not the area!
        private const short CELL_SIZE = 2;    // This is the width or height, not the area!
        private const short RAND_ROOM_ATTEMTS = 5;
        private const short ROOM_PLACE_ATTEMPTS = 10;
        
        public DungeonCell[,] Cells = new DungeonCell[4,4];
        public int widthInCells  = 0;
        public int heightInCells = 0;

        public int[,] GenerateDungeon(int cellWidth, int cellHeight)
        {
            widthInCells = cellWidth;
            heightInCells = cellHeight;

            //Create the cells!
            CreateCellGrid(cellWidth, cellHeight);
            //Place random rooms (run decor and item pass for each room instead of it's own thing)
            PlaceRooms(cellWidth, cellHeight, 2, 6, 1);
            //Create the maze of corridors!
            GenerateMaze(cellWidth, cellHeight, 1);
            //Place entities
            
            //Process lighting? (Perhaps do this at the renderer as no light data will get shipped to it?)
            
            
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
                    Cells[x, y].Tiles[0] = new DungeonTile();
                    Cells[x, y].Tiles[0].SpriteIndex = 1;
                    Cells[x, y].Tiles[1] = new DungeonTile();
                    Cells[x, y].Tiles[2] = new DungeonTile();
                    Cells[x, y].Tiles[3] = new DungeonTile();
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
                }
            }

        }

        private void GenerateMaze(int cellWidth, int cellHeight, int minBorderDist)
        {
            //Utilizing the algorithm shown by Javidx9 we can generate a maze for all tiles not occupied by rooms
            //Please note that we first have to find a start point not already taken by a room.
            //Perhaps a Hunt and Kill method could guarantee that we get all tiles not connected.
            //This could be a simple nested for loop where we check for cells not marked as connected and start a maze there.
            
            //Return to this to look if there is a possible new start point!
            for (int y = minBorderDist; y < cellHeight - minBorderDist; y++)
            {
                for (int x = minBorderDist; x < cellWidth - minBorderDist; x++)
                {
                    if (!Cells[x, y].Connected)
                    {
                        //This tile will be selected for a maze start!
                        //Start the maze with this position!
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