using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    //So .map files are intended to work as a storage place for tiles and their properties, map size and a jumplist for chunk files.
    //unfortunatly this method means that when a chunk gets loaded it's from secondary memory. (perhaps we just load in more at a time?)

    /*  testMap.map file template
        "metaData" : { "mapName":"testMap", "width": 6, "height": 4 },
        "tileAtlas" : 
        [ 
            { "id": 0, "sprName": "spr_void", "collider": true, "lightSource": false }
            { "id": 1, "sprName": "spr_grass_0", "collider": false, "lightSource": false }
            { "id": 2, "sprName": "spr_grass_1", "collider": false, "lightSource": false }
            { "id": 3, "sprName": "spr_boulder_0", "collider": true, "lightSource": false }
            { "id": 4, "sprName": "spr_water", "collider": true, "lightSource": false }
            { "id": 5, "sprName": "spt_str_lantern_0", "collider": true, "lightSource": true }
        ]
        "regions" :
        [
            { "chunkPath": "Regions/chnk_0.chnk" }
            { "chunkPath": "Regions/chnk_1.chnk" }
            { "chunkPath": "Regions/chnk_2.chnk" }
            { "chunkPath": "Regions/chnk_3.chnk" }
            
            { "chunkPath": "Regions/chnk_4.chnk" }
            { "chunkPath": "Regions/chnk_5.chnk" }
            { "chunkPath": "Regions/chnk_6.chnk" }
            { "chunkPath": "Regions/chnk_7.chnk" }
            
            { "chunkPath": "Regions/chnk_8.chnk" }
            { "chunkPath": "Regions/chnk_9.chnk" }
            { "chunkPath": "Regions/chnk_10.chnk" }
            { "chunkPath": "Regions/chnk_11.chnk" }
            
            { "chunkPath": "Regions/chnk_12.chnk" }
            { "chunkPath": "Regions/chnk_13.chnk" }
            { "chunkPath": "Regions/chnk_14.chnk" }
            { "chunkPath": "Regions/chnk_15.chnk" }
            
            { "chunkPath": "Regions/chnk_16.chnk" }
            { "chunkPath": "Regions/chnk_17.chnk" }
            { "chunkPath": "Regions/chnk_18.chnk" }
            { "chunkPath": "Regions/chnk_19.chnk" }

            { "chunkPath": "Regions/chnk_20.chnk" }
            { "chunkPath": "Regions/chnk_21.chnk" }
            { "chunkPath": "Regions/chnk_22.chnk" }
            { "chunkPath": "Regions/chnk_23.chnk" }
        ]
    */
}

public struct Map 
{
    string mapName;
    int width;
    int height;

    List<Atlas.TileObject> tileAtlas;
    string[] chunkPaths;
}
