using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;


namespace EDITOR.MAP
{
    public class EditorChunk : MonoBehaviour
    {
        public Chunk chunkData;
        public Tilemap tilemap;
        List <TileObject> cachedTiles = new List<TileObject>(); 
        private bool dirty = false;
        private bool modifying = false;

        public void OnMouseDown()
        {
            Debug.Log("Caught the player clicking!");
            //Find where the mouse is and if it's within this chunk!
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(mouseWorldPos);

            Vector2 cStartPos = chunkData.GetChunkStartPos();
            if (mouseWorldPos.x >= cStartPos.x && mouseWorldPos.x < cStartPos.x + Chunk.chunkSize &&
                mouseWorldPos.y >= cStartPos.y && mouseWorldPos.x < cStartPos.x + Chunk.chunkSize)
            {
                //We clicked on a tile in the chunk!
                //If the PointerImageGhost selected is not null, rewrite the chunkdata to change that tile
                if (PointerImageGhost.selected.tile != null)
                {
                    dirty = true;
                    modifying = true;
                }
            }

            //Send the data to the EditorMap, set the data returned to chunkData and redraw the chunk!
        }

        public void OnMouseUp()
        {
            modifying = false;
            if (dirty)
                chunkData = transform.parent.GetComponent<EditorMap>().OverwriteChunkDataOnMap(chunkData);
        }

        private void Update()
        {
            if (modifying)
                DrawToChunk();
        }

        private void DrawToChunk()
        {
            //Find where the mouse is and if it's within this chunk!
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(mouseWorldPos);

            Vector2 cStartPos = chunkData.GetChunkStartPos();
            Vector2 localMousePos = mouseWorldPos - cStartPos; //Get the mouse pos relative to the chunk
            Vector2Int roundedPos = new Vector2Int(Mathf.FloorToInt(localMousePos.x), Mathf.FloorToInt(localMousePos.y));

            if (roundedPos.x < 0 || roundedPos.x >= Chunk.chunkSize ||
                roundedPos.y < 0 || roundedPos.y >= Chunk.chunkSize)    return;

            ReplaceTileOnChunk(roundedPos, PointerImageGhost.selected.id);
        }

        private void ReplaceTileOnChunk(Vector2Int tilePos, int idOfNewTile)
        {
            chunkData.mapData[tilePos.x, tilePos.y] = idOfNewTile; //Please work it's like 2am
            tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), TileAtlas.FetchTileObjectByID(idOfNewTile).tile);
        }

        public void DrawChunk()
        {
            TileObject[,] tileData = ConvertToTiles(chunkData.mapData);
            MAP_DISPLAY_MODES currentMode = EditorMap.MAP_DISPLAY_MODE;
            Tile colliderTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
            colliderTile.sprite = Resources.Load<Sprite>("Sprites/UI/spr_ui_trash");
            colliderTile.color = Color.red;

            Debug.Log("Drawing chunk with " + currentMode);

            Vector2Int cStartPos = chunkData.GetChunkStartPos();
            Vector3Int cVec3Pos = new Vector3Int(cStartPos.x, cStartPos.y, 0);
            tilemap.origin = tilemap.WorldToCell(cVec3Pos);
            tilemap.size = new Vector3Int(Chunk.chunkSize, Chunk.chunkSize, 1);
            tilemap.ResizeBounds();
            tilemap.SetTile(tilemap.WorldToCell(cVec3Pos + new Vector3Int(0, 0, 0)), TileAtlas.FetchTileObjectByID(0).tile);
            tilemap.SetTile(tilemap.WorldToCell(cVec3Pos + new Vector3Int(Chunk.chunkSize - 1,Chunk.chunkSize - 1, 0)), TileAtlas.FetchTileObjectByID(0).tile);
            tilemap.ResizeBounds();
            tilemap.CompressBounds();
            tilemap.RefreshAllTiles();
            for (int y = 0; y < Chunk.chunkSize; y++)
            {
                for (int x = 0; x < Chunk.chunkSize; x++)
                {
                    //Debug.Log("X: " + x + " Y: " + y + " TILE: " + tileData[x, y].tile);
                    //DEPENDING ON THE MAP_DISPLAY_MODE, CHECK IF WE'RE GONNA INJECT A SUBSTITUTE TILE!
                    tilemap.SetTile(tilemap.WorldToCell(cVec3Pos + new Vector3Int(x, y, 0)), tileData[x, y].tile);
                    switch (currentMode)
                    {
                        case MAP_DISPLAY_MODES.DEFAULT:
                            continue;

                        case MAP_DISPLAY_MODES.COLLIDER:
                            if (tileData[x, y].collider)
                            {
                                tilemap.SetTile(tilemap.WorldToCell(cVec3Pos + new Vector3Int(x, y, 0)), colliderTile);
                            }
                            continue;
                        
                        case MAP_DISPLAY_MODES.LIGHTS:
                            if (tileData[x, y].lightSource)
                            {
                                tilemap.SetTile(tilemap.WorldToCell(cVec3Pos + new Vector3Int(x, y, 0)), colliderTile);
                            }
                            continue;
                    }
                }
            }
            ResizeColliderBounds(new Vector3(Chunk.chunkSize / 2, Chunk.chunkSize / 2), Chunk.chunkSize, Chunk.chunkSize);
        }

        private TileObject[,] ConvertToTiles(int[,] mapData)
        {
            TileObject[,] toReturn = new TileObject[Chunk.chunkSize, Chunk.chunkSize];

            for (int y = 0; y < Chunk.chunkSize; y++)
            {
                for (int x = 0; x < Chunk.chunkSize; x++)
                {
                    int intData = mapData[x, y];
                    bool foundInCache = false;
                    foreach (var cachedTile in cachedTiles) 
                    {
                        if (cachedTile.id == intData) 
                        { 
                            if (cachedTile.tile == null) { Debug.Log("null cachedTileObject at: " + cachedTile.id); cachedTiles.Remove(cachedTile); break; } //Broken tile, fetch a new one
                            toReturn[x, y] = cachedTile.Copy(); 
                            foundInCache = true; 
                            break; 
                        } 
                    }
                    if (foundInCache) continue;
                    TileObject fetched = TileAtlas.FetchTileObjectByID(intData);
                    toReturn[x, y] = fetched.Copy();
                    cachedTiles.Add(fetched);
                }
            }
            return toReturn;
        }

        public void SetChunkData(Chunk chunk)
        {
            chunkData = chunk;
            DrawChunk();
        }

        private void ResizeColliderBounds(Vector3 center, int width, int height)
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.offset = center;
            col.size = new Vector3(width, height, 0);
        }
    }
}