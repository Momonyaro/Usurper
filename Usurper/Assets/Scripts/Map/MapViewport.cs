using UnityEngine;
using UnityEngine.Tilemaps;

public class MapViewport : MonoBehaviour
{

    public const int viewPortRadius = 16;

    private Vector2 playerPosOnMap = new Vector2(50, 50);

    //Here comes the mess that is tilemaps. We need to create it so that it creates the bounds (63x63 tiles) around the player at 0,0.
    public Tilemap viewport;
    public World loadedWorld = new World();

    private void Start()
    {
        loadedWorld.Init();
    }

    private void OnMapUpdate()
    {
        
    }

    private void viewPortSetData(int[] mapData)
    {

    }

}