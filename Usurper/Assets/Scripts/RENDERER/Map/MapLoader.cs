using System;
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
    public MapExporter mapExporter;
    public MapViewport mapViewport;
    public Text loadingStatusText;
    public InputField customNameField;
    public GameObject mainMenuPanel;
    public GameObject loadingScreenPanel;
    public TileAtlasEditor mapEditor;
    public TileAtlasEditor dngEditor;

    public string customCampaignToLoad = "Usurper";
    //When the renderer starts with inEditor = false, that means we want to load a campaign. We should either fix this through a separate scene or by
    //not displaying a game until a campaign has been loaded through the MapLoader.

    //Before loading the map we need to load sprites.

    private void Start()
    {
        if (customNameField != null) customNameField.SetTextWithoutNotify(customCampaignToLoad);
    }

    public void LoadNewUsurperGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);
        customCampaignToLoad = "Usurper";
        StartCoroutine(LoadCampaignAndResources());
    }

    public void SetCustomCampaignToLoad(string mapName)
    {
        customCampaignToLoad = mapName;
    }

    public void StartCustomCampaign()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);
        StartCoroutine(LoadCampaignAndResources());
    }
    
    public IEnumerator LoadCampaignAndResources()
    {
        Debug.Log(customCampaignToLoad + " | Attempting fetch...");
        if (loadingStatusText != null) loadingStatusText.text = "Loading Sprites...";
        Debug.Log("Initializing resources!");
        yield return new WaitForEndOfFrame();
        if (loadingStatusText != null) loadingStatusText.text = "Loading Map Files...";
        Debug.Log("Loading Map!");
        if (mapExporter.LoadMap(customCampaignToLoad))
        {
            yield return new WaitForEndOfFrame();
            if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);
            if (FindObjectOfType<TileAtlasEditor>()) 
            { 
                Debug.Log("[EDITOR ONLY] Loading the tile editor!"); 
                StartCoroutine(mapEditor.LoadTileAtlasIfReady());
                StartCoroutine(dngEditor.LoadTileAtlasIfReady());
            }
            FindObjectOfType<MapViewport>().initialized = true;
            Debug.Log("Finished Fetch!");

            FindObjectOfType<EntityManager>().UpdatePlayer(Vector2Int.zero);
        }
        else
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);
        }
        
        yield break;
    }
}
