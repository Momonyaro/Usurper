using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RENDERER.UTILS;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;

public class TileAtlasEditor : MonoBehaviour
{
    //On start, load in the tileAtlas.
    //After the import is complete, create the tileAtlas UI objects
    //Allow dataChanges in the UI object and sync them with the atlas

    public Transform blockContentParent;
    public GameObject tileBlockPrefab;
    public GameObject newTileBlockPrefab;
    public bool readDungeonTiles = false;

    private void Start()
    {
        if (readDungeonTiles) StartCoroutine(LoadTileAtlasIfReady());
    }

    public IEnumerator LoadTileAtlasIfReady()
    {
        while (!StreamingResourceLoader.finishedReading) { Debug.Log("Waiting for resource loader..."); yield return null; }

        for (int i = 0; i < blockContentParent.transform.childCount; i++)
        {
            Destroy(blockContentParent.transform.GetChild(i).gameObject);
        }

        //Here we load it and create the UI blocks
        Debug.Log("Reading dungeon tiles ? " + readDungeonTiles);
        List<TileObject> fetchedTiles = (readDungeonTiles) ? TileAtlas.DngTileObjects : TileAtlas.TileObjects;
        for (int i = 0; i < fetchedTiles.Count; i++)
        {
            GameObject tileBlock = Instantiate(tileBlockPrefab, blockContentParent);
            ListTileObjContainer tileContainer = tileBlock.GetComponent<ListTileObjContainer>();
            tileContainer.SetThisTileObj(fetchedTiles[i]);
            tileContainer.dungeonTile = readDungeonTiles;
            tileContainer.parent = this;
            tileContainer.index = i;
        }

        //fetchedTiles.Sort(SortByID);

        GameObject newBtn = Instantiate(newTileBlockPrefab, blockContentParent);
        newBtn.GetComponent<ListTileObjContainer>().parent = this;
        newBtn.GetComponent<ListTileObjContainer>().dungeonTile = readDungeonTiles;

        ToggleTileInteractivity(true);

        if (!readDungeonTiles) FindObjectOfType<MapViewport>().OnMapUpdate();
        
        yield break;
    }

    public void ToggleTileInteractivity(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ListTileObjContainer>() != null)
            {
                ListTileObjContainer container = transform.GetChild(i).GetComponent<ListTileObjContainer>();
                if (container.sprNameField != null) container.sprNameField.interactable = active;
                if (container.tileIdField != null) container.tileIdField.interactable = active;
                if (container.colliderToggle != null) container.colliderToggle.interactable = active;
                if (container.lightSrcToggle != null) container.lightSrcToggle.interactable = active;
                if (container.transparencyToggle != null) container.transparencyToggle.interactable = active;
            }
        }
    }
}
