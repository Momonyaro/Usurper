using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS;
using RENDERER.MAP;
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

    public IEnumerator LoadCampaignAndResources(string campaingName)
    {
        if (loadingStatusText != null) loadingStatusText.text = "Loading Sprites...";
        resourceLoader.Init();
        yield return new WaitForEndOfFrame();
        if (loadingStatusText != null) loadingStatusText.text = "Loading Map Files...";
        mapExporter.LoadMap(campaingName);
        yield return new WaitForEndOfFrame();
        mapViewport.OnMapUpdate();
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);

        yield break;
    }
}
