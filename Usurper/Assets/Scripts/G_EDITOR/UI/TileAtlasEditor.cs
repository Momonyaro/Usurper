using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RENDERER.UTILS;
using RENDERER.UTILS.Atlas;

public class TileAtlasEditor : MonoBehaviour
{
    //On start, load in the tileAtlas.
    //After the import is complete, create the tileAtlas UI objects
    //Allow dataChanges in the UI object and sync them with the atlas

    public Transform blockContentParent;
    public GameObject tileBlockPrefab;
    public GameObject newTileBlockPrefab;

    private void Start()
    {
        StartCoroutine(LoadTileAtlasIfReady());
    }

    public IEnumerator LoadTileAtlasIfReady()
    {
        while (!StreamingResourceLoader.finishedReading) { yield return null; }

        //Here we load it and create the UI blocks
        List<TileObject> fetchedTiles = TileAtlas.tileObjects;
        for (int i = 0; i < fetchedTiles.Count; i++)
        {
            GameObject tileBlock = Instantiate(tileBlockPrefab, blockContentParent);
            ListTileObjContainer tileContainer = tileBlock.GetComponent<ListTileObjContainer>();
            tileContainer.SetThisTileObj(fetchedTiles[i]);
        }

        fetchedTiles.Sort(SortByID);

        Instantiate(newTileBlockPrefab, blockContentParent);

        ToggleTileInteractivity();
        
        yield break;
    }

    public void ToggleTileInteractivity()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ListTileObjContainer>() != null)
            {
                ListTileObjContainer container = transform.GetChild(i).GetComponent<ListTileObjContainer>();
                container.sprNameField.interactable = !container.lightSrcToggle.interactable;
                container.tileIdField.interactable = !container.lightSrcToggle.interactable;
                container.colliderToggle.interactable = !container.lightSrcToggle.interactable;
                container.lightSrcToggle.interactable = !container.lightSrcToggle.interactable;
            }
        }
    }

    static int SortByID(TileObject t1, TileObject t2)
    {
        return t2.id.CompareTo(t1.id);
    }
}
