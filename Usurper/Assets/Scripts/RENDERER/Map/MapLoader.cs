using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RENDERER.UTILS.Atlas;

public class MapLoader : MonoBehaviour
{
    //When the renderer starts with inEditor = false, that means we want to load a campaign. We should either fix this through a separate scene or by
    //not displaying a game until a campaign has been loaded through the MapLoader.
}

public struct Map 
{
    string mapName;
    int width;
    int height;

    List<TileObject> tileAtlas;
    string[] chunkPaths;
}
