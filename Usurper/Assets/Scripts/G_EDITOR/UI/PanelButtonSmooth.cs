using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelButtonSmooth : MonoBehaviour
{
    public bool panelOpen = false;
    public RectTransform panelTransform;
    public TileAtlasEditor tempSolutionTileAtlasToggle;
    public float panelMoveDist;
    public bool dngBool = false;

    private void Update()
    {
        //Retard alert! Bad Programming!
        GetComponent<Button>().interactable = true;
        if (ViewModeFlipper.EDITOR_VIEW_MODE == EDITOR_VIEW_MODES.MAP_VIEW_MODE && dngBool)
            GetComponent<Button>().interactable = false;
        else if (ViewModeFlipper.EDITOR_VIEW_MODE == EDITOR_VIEW_MODES.DUNGEON_VIEW_MODE && !dngBool)
            GetComponent<Button>().interactable = false;
    }

    public void TogglePanelSmooth(float smoothSpeed)
    {
        if (panelOpen)
        {
            StartCoroutine(MovePanel(panelTransform.localPosition + new Vector3(panelMoveDist, 0, 0), smoothSpeed));
            if (tempSolutionTileAtlasToggle != null) tempSolutionTileAtlasToggle.ToggleTileInteractivity(!panelOpen);
        }
        else 
        {
            StartCoroutine(MovePanel(panelTransform.localPosition + new Vector3(-panelMoveDist, 0, 0), smoothSpeed));
            if (tempSolutionTileAtlasToggle != null) tempSolutionTileAtlasToggle.ToggleTileInteractivity(!panelOpen);
        }
    }

    public IEnumerator MovePanel(Vector3 targetPosition, float smoothSpeed)
    {
        transform.GetChild(0).rotation = panelOpen ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 90);
        while (Vector3.Distance(panelTransform.localPosition, targetPosition) > .1f)
        {
            panelTransform.localPosition = Vector3.Slerp(panelTransform.localPosition, targetPosition, smoothSpeed);
            yield return null;
        }

        panelOpen = !panelOpen;
        yield break;
    }
}
