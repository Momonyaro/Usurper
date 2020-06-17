using UnityEngine;
using UnityEngine.Tilemaps;
using Atlas;
using System.Collections.Generic;

public class MapViewport : MonoBehaviour
{

    public const int viewPortRadius = 31;

    [SerializeField]
    private Vector2Int playerPosOnMap = new Vector2Int(0, 0);
    private int[,] viewportMapData;
    [SerializeField]
    public List<TileObject> cachedTiles;

    //Here comes the mess that is tilemaps. We need to create it so that it creates the bounds (63x63 tiles) around the player at 0,0.
    public Tilemap viewport;
    public World loadedWorld = new World();
    private MapLighter mapLighter;

    private void Start()
    {
        FindObjectOfType<StreamingResourceLoader>().Init();
        loadedWorld.Init();
        mapLighter = new MapLighter();
        OnMapUpdate();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.W)) { playerPosOnMap += Vector2Int.up; OnMapUpdate();}
            else if (Input.GetKeyDown(KeyCode.A)) { playerPosOnMap += Vector2Int.left; OnMapUpdate();}
            else if (Input.GetKeyDown(KeyCode.S)) { playerPosOnMap += Vector2Int.down; OnMapUpdate();}
            else if (Input.GetKeyDown(KeyCode.D)) { playerPosOnMap += Vector2Int.right; OnMapUpdate();}
        }
    }

    private void OnMapUpdate()
    {
        DrawTileArrayToTilemap(ConvertToTiles(loadedWorld.GetWorldDataAtPoint(playerPosOnMap)));
    }

    private void viewPortSetData(int[,] mapData)
    {
        this.viewportMapData = mapData;
    }

    //Translate the intData to tiles.
    private TileObject[,] ConvertToTiles(int[,] mapData)
    {
        TileObject[,] toReturn = new TileObject[31, 31];

        cachedTiles = new List<TileObject>(); 

            for (int y = 0; y < 31; y++)
            {
                for (int x = 0; x < 31; x++)
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
        viewport.size = viewport.WorldToCell(new Vector3Int(31, 31, 1));
        viewport.ResizeBounds();
        viewport.SetTile(new Vector3Int(-15, -15, 0), null);
        viewport.SetTile(new Vector3Int( 15,  15, 0), null);
        viewport.ResizeBounds();
        viewport.RefreshAllTiles();
        Vector3Int[,] positions = new Vector3Int[31, 31];
        for (int y = -15; y < 16; y++)
        {
            for (int x = -15; x < 16; x++)
            {
                positions[x + 15, y + 15] = new Vector3Int(x, y, 0);
                //Debug.Log(positions[i]);
            }
        }

        tileData[15, 15].lightSource = true;
        tileData[15, 15].tile.sprite = Atlas.SpriteAtlas.FetchSpriteByName("spr_player");
        tileData = mapLighter.LightPass(tileData, 7);

        Debug.Log("Placing " + tileData.Length + " tiles on viewport...");

        for (int y = 0; y < viewPortRadius; y++)
        {
            for (int x = 0; x < viewPortRadius; x++)
            {
                viewport.SetTile(viewport.WorldToCell(positions[x, y]), tileData[x, y].tile);
            }
        }

        viewport.RefreshAllTiles();
    }

}