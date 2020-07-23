using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RULESET.WORLD
{
    public class PathfinderAlgorithm : DungeonAlgorithm
    {

        private const int minRoomSize = 4;
        private const int maxRoomSize = 9;
        private const int corridorWidth = 1;
        private const int minBorderDist = 2;
        private const int minNewDoorPosAttempts = 10;

        private Stack<PRect> dungeonStack = new Stack<PRect>();
        private int width;
        private int height;

        public override int[,] Generate(int width, int height)
        {
            dungeonStack.Clear();
            this.width = width; this.height = height;
            CreateCanvas(width, height);

            RandomlyPlaceStartRoom(width, height);

            RecursiveRoomPlacementLoop(10);

            return ConvertToIntData(width, height);
        }

        //The pathfinder algorithm is where we place a room, decorate it, check if we can build a corridor of x length
        //then build that corridor and place a door at the room border. Then we check random positions at the corridor
        //for a new place to build a room. If we can't place a new corridor in a room we go back to the old corridor
        // and check if we can build a room in a different direction.

        //We can store each corridor and room as a rect for recursion.

        //basically, place a room or corridor, then check if we can place another one. If we can't, go back to then one before and try there.
        //then we do this until we reach the beginning, after which we say we're done if that fails as well.

        private void RandomlyPlaceStartRoom(int width, int height)
        {
            Vector2Int size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize));
            Vector2Int pos = new Vector2Int(Random.Range(minBorderDist, width - size.x - minBorderDist), Random.Range(minBorderDist, height - size.y - minBorderDist));
            PRect startRoom = new PRect(new RectInt(pos, size), true);

            for (int y = 0; y < startRoom.rect.height; y++)
            {
                for (int x = 0; x < startRoom.rect.width; x++)
                {
                    Tiles[startRoom.rect.x + x, startRoom.rect.y + y] = TILE_TYPES.FLOOR;
                }
            }
            
            dungeonStack.Push(startRoom);

            //Place the entrance!
            Vector2Int entrancePos = new Vector2Int(Random.Range(pos.x, pos.x + size.x), Random.Range(pos.y, pos.y + size.y));
            bool placeAtY = (Random.value > 0.6) ? true : false; // if true, we lock x to either width or zero and y is free between the range
            if (placeAtY) entrancePos = ((entrancePos.x - pos.x) * 2 >= size.x) ? new Vector2Int(pos.x + size.x, entrancePos.y) : new Vector2Int(pos.x - 1, entrancePos.y);
            else entrancePos = ((entrancePos.y - pos.y) * 2 >= size.y) ? new Vector2Int(entrancePos.x, pos.y + size.y) : new Vector2Int(entrancePos.x, pos.y - 1);

            Tiles[entrancePos.x, entrancePos.y] = TILE_TYPES.ENTRANCE;
        }

        private void RecursiveRoomPlacementLoop(int attemptsPerRoom)
        {
            //Pick an area within the map, attempt to place a room there. If we fail x number of times,
            //pop the current room and try an earlier position. (if the start room is popped, break!)

            while (dungeonStack.Count > 0)
            {
                PRect currentRoom = dungeonStack.Peek();

                int q = attemptsPerRoom;
                bool failedToPlace = false;
                while (q > 0)
                {
                    Vector2Int size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize));

                    bool makeRoom = (Random.value < 0.5);
                    if (!makeRoom) size = (Random.value > 0.5) ? new Vector2Int(corridorWidth, size.y) : new Vector2Int(size.x, corridorWidth);
                    PRect newRoom = new PRect(new RectInt(new Vector2Int(0, 0), size), makeRoom);

                    Vector3Int doorData = GetPossibleExpansionDirection(currentRoom);
                    Vector2Int dir = new Vector2Int(0, 0);

                    switch (doorData.z)
                    {
                        case 0:
                            dir = Vector2Int.right;
                            break;
                        case 1:
                            dir = Vector2Int.left;
                            break;
                        case 2:
                            dir = Vector2Int.up;
                            break;
                        case 3:
                            dir = Vector2Int.down;
                            break;
                        case -1:
                            q--;
                            failedToPlace = true;
                            continue;
                    }

                    failedToPlace = !CheckArea(newRoom, new Vector2Int(doorData.x, doorData.y), dir, out newRoom);

                    if (!failedToPlace)
                    {
                        //Now we can place the room / corridor!
                        if (!currentRoom.room && !newRoom.room) Tiles[doorData.x, doorData.y] = TILE_TYPES.FLOOR;
                        else Tiles[doorData.x, doorData.y] = TILE_TYPES.DOOR;
                        PlaceRoom(newRoom);
                        dungeonStack.Push(newRoom);
                        Debug.Log("doorPos: " + doorData + " roomPos: " + newRoom.rect + " pos delta:" + new Vector2Int(newRoom.rect.x - doorData.x, newRoom.rect.y - doorData.y));
                        return; //break;
                    }
                    else q--;
                }

                if (failedToPlace) {dungeonStack.Pop();}
            }
        }

        private Vector3Int GetPossibleExpansionDirection(PRect currentRoom)
        {
            Vector2Int pos = new Vector2Int(currentRoom.rect.x, currentRoom.rect.y);
            Vector2Int size = new Vector2Int(currentRoom.rect.width, currentRoom.rect.height);
            Vector2Int center = new Vector2Int(pos.x + Mathf.FloorToInt(size.x / 2.0f), pos.y + Mathf.FloorToInt(size.y / 2.0f));

            Vector2Int doorPos = new Vector2Int(Random.Range(pos.x, pos.x + size.x - 1), Random.Range(pos.y, pos.y + size.y - 1));

            bool okPos = false;
            for (int i = 0; i < minNewDoorPosAttempts; i++)
            {
                bool placeAtY = (Random.value > 0.6); // if true, we lock x to either width or zero and y is free between the range
                if (placeAtY) doorPos = ((doorPos.x - pos.x) * 2 >= size.x) ? new Vector2Int(pos.x + size.x, doorPos.y) : new Vector2Int(pos.x - 1, doorPos.y);
                else doorPos = ((doorPos.y - pos.y) * 2 >= size.y) ? new Vector2Int(doorPos.x, pos.y + size.y) : new Vector2Int(doorPos.x, pos.y - 1);

                if (doorPos.x < 0 || doorPos.x >=  width) { continue; }
                if (doorPos.y < 0 || doorPos.y >= height) { continue; }

                if (Tiles[doorPos.x, doorPos.y] != TILE_TYPES.WALL) { continue; }

                okPos = true;
                break;
            }

            int dir = 0;
            int deltaX = center.x - doorPos.x;
            int deltaY = center.y - doorPos.y;
            bool xLongest = Mathf.Abs(deltaX) >= Mathf.Abs(deltaY);
            //We want to return an int in the range of 0 - 3
            if      ( xLongest && deltaX < 0) dir = 0;
            else if ( xLongest && deltaX > 0) dir = 1;
            else if (!xLongest && deltaY < 0) dir = 2;
            else if (!xLongest && deltaY > 0) dir = 3;

            if (!okPos) dir = -1;
            
            return new Vector3Int(doorPos.x, doorPos.y, dir);
        }

        private bool CheckArea(PRect pRect, Vector2Int doorPos, Vector2Int direction, out PRect room)
        {
            //check if the area is free. Return true if it is, return false if something is blocking.

            //We can shift the room pos some random number before attempting to place it.
            //We do need to also place the room based on a offset from the doorpos and direction.

            
            //The problem seems to occur one above when going left && one left when going up
            
            
            int xRoomOffset = 0; //Random.Range(0, pRect.rect.width - 1);
            int yRoomOffset = 0; //Random.Range(0, pRect.rect.height - 1);

            if (direction.Equals(Vector2Int.right))
            {
                pRect.rect.x = doorPos.x + 1;
                pRect.rect.y = doorPos.y - yRoomOffset;
            }
            else if (direction.Equals(Vector2Int.left))
            {
                pRect.rect.x = doorPos.x - pRect.rect.width;
                pRect.rect.y = doorPos.y - yRoomOffset;
            }
            else if (direction.Equals(Vector2Int.up))
            {
                pRect.rect.x = doorPos.x - xRoomOffset;
                pRect.rect.y = doorPos.y + 1;
            }
            else if (direction.Equals(Vector2Int.down))
            {
                pRect.rect.x = doorPos.x - xRoomOffset;
                pRect.rect.y = doorPos.y - pRect.rect.height;
            }
            else Debug.Log("Wierd direction found, check it out! | " + direction);

            RectInt areaToCheck = new RectInt(new Vector2Int(pRect.rect.x - 1, pRect.rect.y - 1),
                new Vector2Int(pRect.rect.width + 1, pRect.rect.height + 1));

            room = pRect;

            for (int y = 0; y < areaToCheck.height + 1; y++)
                for (int x = 0; x < areaToCheck.width + 1; x++)
                {
                    if (areaToCheck.x + x < 0 || areaToCheck.x + x >= width)  { 
                        return false;
                    }
                    if (areaToCheck.y + y < 0 || areaToCheck.y + y >= height) { 
                        return false;
                    }

                    if (Tiles[areaToCheck.x + x, areaToCheck.y + y] == TILE_TYPES.FLOOR) { 
                        return false;
                    }
                    if (Tiles[areaToCheck.x + x, areaToCheck.y + y] == TILE_TYPES.ENTRANCE) { 
                        return false;
                    }
                }
            
            return true;
        }

        private void PlaceRoom(PRect pRect)
        {
            //Draw the rect data to the tiles, add the room to the stack.
            for (int y = 0; y < pRect.rect.height; y++)
                for (int x = 0; x < pRect.rect.width; x++)
                {
                    Tiles[pRect.rect.x + x, pRect.rect.y + y] = TILE_TYPES.FLOOR;
                }
        }

        private struct PRect
        {
            public bool room;
            public RectInt rect;

            public PRect(RectInt rect, bool room)
            {
                this.rect = rect;
                this.room = room;
            }
        }
    }
}
