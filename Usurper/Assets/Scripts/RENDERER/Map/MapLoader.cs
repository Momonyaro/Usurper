using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS;
using RENDERER.MAP;
using RULESET.MANAGERS;
using EDITOR.EXPORT;

public class MapLoader : MonoBehaviour
{
    public StreamingResourceLoader resourceLoader;
    public MapExporter mapExporter;
    public MapViewport mapViewport;
    public Text loadingStatusText;
    public GameObject mainMenuPanel;
    public GameObject loadingScreenPanel;
    //When the renderer starts with inEditor = false, that means we want to load a campaign. We should either fix this through a separate scene or by
    //not displaying a game until a campaign has been loaded through the MapLoader.

    //Before loading the map we need to load sprites.

    public void LoadNewUsurperGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);
        StartCoroutine(LoadCampaignAndResources("Usurper"));
    }

    public IEnumerator LoadCampaignAndResources(string campaignName)
    {
        Debug.Log(campaignName + " | Attempting fetch...");
        if (loadingStatusText != null) loadingStatusText.text = "Loading Sprites...";
        Debug.Log("Initializing resources!");
        resourceLoader.Init();
        yield return new WaitForEndOfFrame();
        if (loadingStatusText != null) loadingStatusText.text = "Loading Map Files...";
        Debug.Log("Loading Map!");
        mapExporter.LoadMap(campaignName);
        yield return new WaitForEndOfFrame();
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);
        if (FindObjectOfType<TileAtlasEditor>()) { Debug.Log("[EDITOR ONLY] Loading the tile editor!"); StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady()); }
        FindObjectOfType<MapViewport>().initialized = true;
        Debug.Log("Finished Fetch!");

        FindObjectOfType<EntityManager>().UpdatePlayer(Vector2Int.zero);
        yield break;
    }
}
