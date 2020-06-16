using UnityEngine;
using UnityEngine.Tilemaps;
using Atlas;
using System.Collections.Generic;

public class MapViewport : MonoBehaviour
{

    public const int viewPortRadius = 15;

    private Vector2Int playerPosOnMap = new Vector2Int(0, 0);
    private int[] viewportMapData;
    [SerializeField]
    public List<TileObject> cachedTiles;

    //Here comes the mess that is tilemaps. We need to create it so that it creates the bounds (63x63 tiles) around the player at 0,0.
    public Tilemap viewport;
    public World loadedWorld = new World();

    private void Start()
    {
        FindObjectOfType<StreamingResourceLoader>().Init();
        loadedWorld.Init();
        OnMapUpdate();
    }

    private void OnMapUpdate()
    {
        DrawTileArrayToTilemap(ConvertToTiles(loadedWorld.GetWorldDataAtPoint(playerPosOnMap)));
    }

    private void viewPortSetData(int[] mapData)
    {
        this.viewportMapData = mapData;
    }

    //Translate the intData to tiles.
    private TileObject[] ConvertToTiles(int[] mapData)
    {
        TileObject[] toReturn = new TileObject[mapData.Length];

        cachedTiles = new List<TileObject>(); 

        for (int i = 0; i < mapData.Length; i++)
        {
            int intData = mapData[i];
            bool foundInCache = false;
            foreach (var cachedTile in cachedTiles) 
            {
                if (cachedTile.id == intData) 
                { 
                    if (cachedTile.tile == null) { Debug.Log("null cachedTileObject at: " + cachedTile.id); cachedTiles.Remove(cachedTile); break; } //Broken tile, fetch a new one
                    toReturn[i] = cachedTile.Copy(); 
                    foundInCache = true; 
                    break; 
                } 
            }
            if (foundInCache) continue;

            TileObject fetched = TileAtlas.FetchTileObjectByID(intData);
            toReturn[i] = fetched.Copy();
            cachedTiles.Add(fetched);
        }

        return toReturn;
    }

    public void DrawTileArrayToTilemap(TileObject[] tileData)
    {
        viewport.size = viewport.WorldToCell(new Vector3Int(31, 31, 1));
        viewport.ResizeBounds();
        viewport.SetTile(new Vector3Int(-15, -15, 0), null);
        viewport.SetTile(new Vector3Int( 15,  15, 0), null);
        viewport.ResizeBounds();
        viewport.RefreshAllTiles();
        Vector3Int[] positions = new Vector3Int[31 * 31];
        int i = 0;
        for (int y = -15; y < 16; y++)
        {
            for (int x = -15; x < 16; x++)
            {
                positions[i] = new Vector3Int(playerPosOnMap.x + x, playerPosOnMap.y + y, 0);
                //Debug.Log(positions[i]);
                i++;
            }
        }
        Debug.Log("Placing " + tileData.Length + " tiles on viewport...");

        for (int j = 0; j < tileData.Length; j++)
        {
            //if (tileData[j] == null) Debug.Log("null tile at:" + viewport.WorldToCell(positions[j]), tileData[j]);
            //Apply light color in steps. example is 4 levels of brightness where we just take some distance and in steps apply lightlevels.
            viewport.SetTile(viewport.WorldToCell(positions[j]), tileData[j].tile);
        }

        viewport.RefreshAllTiles();
    }

}