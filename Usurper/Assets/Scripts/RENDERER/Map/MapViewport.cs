using UnityEngine;
using UnityEngine.Tilemaps;
using RENDERER.UTILS.Atlas;
using RENDERER.UTILS;
using System.Collections.Generic;



namespace RENDERER.MAP
{

    public enum MAP_DISPLAY_MODES
    {
        DEFAULT,    //Draw the imported data as usual
        COLLIDER,   //Here we draw the tiles with colliders as a Cross and the others as void
        LIGHTS,
    }


    public enum MAP_BOUNDS_MODIFY_DIRECTIONS
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public enum EDITOR_BRUSH_MODES
    {
        PEN,
        CHUNK_FILL
    }

    public class MapViewport : MonoBehaviour
    {

        public static int viewPortRadius = 37;
        private const bool RENDER_EDITOR = false;

        [SerializeField]
        public Vector2Int playerPosOnMap = new Vector2Int(0, 0);
        private int[,] viewportMapData;
        [SerializeField]
        public List<TileObject> cachedTiles;
        public static MAP_DISPLAY_MODES MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.DEFAULT;
        public static EDITOR_BRUSH_MODES EDITOR_BRUSH_MODE = EDITOR_BRUSH_MODES.PEN;

        //Here comes the mess that is tilemaps. We need to create it so that it creates the bounds (63x63 tiles) around the player at 0,0.
        public Tilemap viewport;
        public Tilemap entityViewport;
        public World loadedWorld = new World();
        private MapLighter mapLighter;
        private Sprite cross;
        public bool inEditor = false;
        private bool drawing = false;

        private void Start()
        {
            mapLighter = new MapLighter();
            cross = Resources.Load<Sprite>("Sprites/UI/spr_ui_trash");

            if (inEditor)
            {
                FindObjectOfType<StreamingResourceLoader>().Init();
                viewPortRadius = 61;
                loadedWorld.Init();
                OnMapUpdate();
            } 
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                if (inEditor)
                {
                    //PLEASE CHANGE THIS SHIT LATER :( IT LOOKS TERRIBLE!
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (Input.GetKeyDown(KeyCode.W)) { playerPosOnMap += Vector2Int.up * Chunk.chunkSize; OnMapUpdate(); return; }
                            else if (Input.GetKeyDown(KeyCode.A)) { playerPosOnMap += Vector2Int.left * Chunk.chunkSize; OnMapUpdate(); return; }
                            else if (Input.GetKeyDown(KeyCode.S)) { playerPosOnMap += Vector2Int.down * Chunk.chunkSize; OnMapUpdate(); return; }
                            else if (Input.GetKeyDown(KeyCode.D)) { playerPosOnMap += Vector2Int.right * Chunk.chunkSize; OnMapUpdate(); return; }
                        }
                        else if (Input.GetKeyDown(KeyCode.W)) { playerPosOnMap += Vector2Int.up * 10; OnMapUpdate(); return; }
                        else if (Input.GetKeyDown(KeyCode.A)) { playerPosOnMap += Vector2Int.left * 10; OnMapUpdate(); return; }
                        else if (Input.GetKeyDown(KeyCode.S)) { playerPosOnMap += Vector2Int.down * 10; OnMapUpdate(); return; }
                        else if (Input.GetKeyDown(KeyCode.D)) { playerPosOnMap += Vector2Int.right * 10; OnMapUpdate(); return; }
                    }
                }
                    if (Input.GetKey(KeyCode.W)) { playerPosOnMap += Vector2Int.up; OnMapUpdate(); return; }
                    else if (Input.GetKey(KeyCode.A)) { playerPosOnMap += Vector2Int.left; OnMapUpdate(); return; }
                    else if (Input.GetKey(KeyCode.S)) { playerPosOnMap += Vector2Int.down; OnMapUpdate(); return; }
                    else if (Input.GetKey(KeyCode.D)) { playerPosOnMap += Vector2Int.right; OnMapUpdate(); return; }
            }

            if (drawing)
            {
                DrawOnMap();
            }
        }

        private void OnMouseDown()
        {
            if (inEditor && PointerImageGhost.selected.tile != null)
            {
                drawing = true;
            }

        }

        private void OnMouseUp()
        {
            if (drawing) OnMapUpdate();
            drawing = false;
        }

        private void DrawOnMap()
        {
            //Find where the mouse is and if it's within this chunk!
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(mouseWorldPos);
            foreach (var chunk in loadedWorld.worldData)
            {
                Vector2 cStartPos = chunk.GetChunkStartPos();
                Vector2 localMousePos = (playerPosOnMap + mouseWorldPos + new Vector2(.5f, .5f)) - cStartPos; //Get the mouse pos relative to the chunk
                Debug.Log(localMousePos);
                Vector2Int roundedPos = new Vector2Int(Mathf.FloorToInt(localMousePos.x), Mathf.FloorToInt(localMousePos.y));
    
                if (roundedPos.x < 0 || roundedPos.x >= Chunk.chunkSize ||
                    roundedPos.y < 0 || roundedPos.y >= Chunk.chunkSize)    continue;
    
                Vector3Int toVec3 = new Vector3Int(Mathf.FloorToInt(mouseWorldPos.x + 0.5f), Mathf.FloorToInt(mouseWorldPos.y + 0.5f), 0);
                toVec3 = viewport.WorldToCell(toVec3);
                
                switch (EDITOR_BRUSH_MODE)
                {
                    case EDITOR_BRUSH_MODES.PEN:
                        ReplaceTileOnChunk(chunk, roundedPos, toVec3, PointerImageGhost.selected.id);
                    break;

                    case EDITOR_BRUSH_MODES.CHUNK_FILL:
                        ReplaceChunkData(chunk, roundedPos, PointerImageGhost.selected.id);
                    break;
                }

                break;
            }
        }

        private void ReplaceChunkData(Chunk chunkData, Vector2Int roundedPos, int idOfNewTile)
        {
            int idToReplace = chunkData.mapData[roundedPos.x, roundedPos.y];
            for (int y = 0; y < Chunk.chunkSize; y++)
            {
                for (int x = 0; x < Chunk.chunkSize; x++)
                {
                    if (chunkData.mapData[x, y] == idToReplace)
                        chunkData.mapData[x, y] = idOfNewTile;
                }
            }
            OnMapUpdate();
            drawing = false;
        }

        private void ReplaceTileOnChunk(Chunk chunkData, Vector2Int tilePos, Vector3Int viewPortPos, int idOfNewTile)
        {
            chunkData.mapData[tilePos.x, tilePos.y] = idOfNewTile; //Please work it's like 2am
            viewport.SetTile(viewPortPos, TileAtlas.FetchTileObjectByID(idOfNewTile).tile);
        }

        public void OnMapUpdate()
        {
            DrawTileArrayToTilemap(ConvertToTiles(loadedWorld.GetWorldDataAtPoint(playerPosOnMap)));
            SetColliderBounds();
        }

        public void SetBrushMode(int newMode)
        {
            EDITOR_BRUSH_MODE = (EDITOR_BRUSH_MODES)newMode;
        }

        private void viewPortSetData(int[,] mapData)
        {
            this.viewportMapData = mapData;
        }

        //Translate the intData to tiles.
        private TileObject[,] ConvertToTiles(int[,] mapData)
        {
            TileObject[,] toReturn = new TileObject[viewPortRadius, viewPortRadius];

            cachedTiles = new List<TileObject>(); 

                for (int y = 0; y < viewPortRadius; y++)
                {
                    for (int x = 0; x < viewPortRadius; x++)
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

        public void DrawTileArrayToTilemap(TileObject[,] tileData)
        {
            Tile crossTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
            Tile editorCrossTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
            crossTile.sprite = cross;
            editorCrossTile.sprite = cross;
            crossTile.color = Color.red;
            int halfWidth = ((viewPortRadius - 1) / 2);
            viewport.size = viewport.WorldToCell(new Vector3Int(viewPortRadius, viewPortRadius, 2));
            entityViewport.size = viewport.WorldToCell(new Vector3Int(viewPortRadius, viewPortRadius, 2));
            viewport.ResizeBounds();
            entityViewport.ResizeBounds();

            tileData[halfWidth, halfWidth].lightSource = true;
            
            if (inEditor)
            {
                tileData[halfWidth, halfWidth].tile = editorCrossTile;
                tileData[halfWidth, halfWidth].tile.color = Color.blue;
                tileData[halfWidth, halfWidth].lightSource = false;
            }

            if (tileData[halfWidth, halfWidth].tile.sprite == null) tileData[halfWidth, halfWidth].tile.sprite = Resources.Load<Sprite>("Sprites/spr_err");
            if (!inEditor) tileData = mapLighter.LightPass(tileData, 8);

            Debug.Log("Placing " + tileData.Length + " tiles on viewport...");
            Tile playerTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
            playerTile.sprite = SpriteAtlas.FetchSpriteByName("spr_human_commoner_0");
            entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(0, 0, 1)), playerTile);

            for (int y = 0; y < viewPortRadius; y++)
            {
                for (int x = 0; x < viewPortRadius; x++)
                {
                    viewport.SetTile(viewport.WorldToCell(new Vector3Int(x - halfWidth, y - halfWidth, 0)), tileData[x, y].tile);
                    entityViewport.SetColor(entityViewport.WorldToCell(new Vector3Int(x - halfWidth, y - halfWidth, 0)), tileData[x, y].tile.color);
                    switch (MAP_DISPLAY_MODE)
                    {
                        case MAP_DISPLAY_MODES.COLLIDER:
                            if (tileData[x, y].collider) viewport.SetTile(viewport.WorldToCell(new Vector3Int(x - halfWidth, y - halfWidth, 0)), crossTile);
                        break;

                        case MAP_DISPLAY_MODES.LIGHTS:
                            if (tileData[x, y].lightSource) viewport.SetTile(viewport.WorldToCell(new Vector3Int(x - halfWidth, y - halfWidth, 0)), crossTile);
                        break;
                    }
                    
                }
            }

            viewport.RefreshAllTiles();
        }

        public void ResizeEditorMapBounds(int value, MAP_BOUNDS_MODIFY_DIRECTIONS direction)
        {
            Debug.Log(value + " " + direction);
            List<Chunk> newChunks = new List<Chunk>();
            //Resize map size
            switch (direction)
            {
                case MAP_BOUNDS_MODIFY_DIRECTIONS.UP: World.height += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN: World.height += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT: World.width += value; break;
                case MAP_BOUNDS_MODIFY_DIRECTIONS.RIGHT: World.width += value; break;
            }

            //a positive down or left value means, shift all chunks up by a chunkSize and create new chunks at 0 x or y
            //a negative down or left means, remove chunks at 0 x or y and shift remaining chunks down by a chunkSize
            if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN || direction == MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT)
            {
                for (int i = 0; i < loadedWorld.worldData.Count; i++)
                {
                    Vector2Int cStartPos = loadedWorld.worldData[i].GetChunkStartPos();
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN) cStartPos += new Vector2Int(0, value * Chunk.chunkSize);
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.LEFT) cStartPos += new Vector2Int(value * Chunk.chunkSize, 0);
                    loadedWorld.worldData[i].SetChunkStartPos(cStartPos.x, cStartPos.y);
                }

                // Remove chunks with positions less than 0
                if (value < 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN)
                    {
                        for (int x = 0; x < World.width; x++)
                        {
                            foreach (var chunk in loadedWorld.worldData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.y < 0)
                                {
                                    loadedWorld.worldData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < World.height; y++)
                        {
                            foreach (var chunk in loadedWorld.worldData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.x < 0)
                                {
                                    loadedWorld.worldData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                }

                //Here we add the extra chunks to fill out the new bounds
                if (value > 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.DOWN)
                    {
                        for (int x = 0; x < World.width; x++) { Chunk cData = new Chunk(x * Chunk.chunkSize, 0); loadedWorld.worldData.Add(cData); }
                    }
                    else 
                    {
                        for (int y = 0; y < World.height; y++) { Chunk cData = new Chunk(0, y * Chunk.chunkSize); loadedWorld.worldData.Add(cData); }
                    }
                }
            }

            //a positive up or right value means, add a extra row or column of chunks for the desired direction
            //a negative up or right means, remove the row or column of chunks so that it correctly represents the width and height of the map.
            if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP || direction == MAP_BOUNDS_MODIFY_DIRECTIONS.RIGHT)
            {
                if (value < 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP)
                    {
                        for (int x = 0; x < World.width; x++)
                        {
                            foreach (var chunk in loadedWorld.worldData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.y >= (World.height) * Chunk.chunkSize)
                                {
                                    loadedWorld.worldData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < World.height; y++)
                        {
                            foreach (var chunk in loadedWorld.worldData)
                            {
                                Vector2Int cStartPos = chunk.GetChunkStartPos();
                                if (cStartPos.x >= (World.width) * Chunk.chunkSize)
                                {
                                    loadedWorld.worldData.Remove(chunk);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (value > 0)
                {
                    if (direction == MAP_BOUNDS_MODIFY_DIRECTIONS.UP)
                    {
                        for (int x = 0; x < World.width; x++) 
                        { 
                            Chunk cData = new Chunk(x * Chunk.chunkSize, (World.height - 1) * Chunk.chunkSize); 
                            newChunks.Add(cData);
                        }
                    }
                    else 
                    {
                        for (int y = 0; y < World.height; y++) 
                        { 
                            Chunk cData = new Chunk((World.width - 1) * Chunk.chunkSize, y * Chunk.chunkSize); 
                            newChunks.Add(cData);
                        }
                    }
                }
                loadedWorld.worldData.AddRange(newChunks);
            }
        }

        public void SetColliderBounds()
        {
            if (GetComponent<BoxCollider2D>() != null)
            {
                BoxCollider2D col = GetComponent<BoxCollider2D>();
                //col.offset = new Vector2(viewPortRadius / 2f, viewPortRadius / 2f);
                col.size = new Vector2(viewPortRadius, viewPortRadius);
            }
        }

    }
}

