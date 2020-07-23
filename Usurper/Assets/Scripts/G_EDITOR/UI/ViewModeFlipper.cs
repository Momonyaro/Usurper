using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EDITOR_VIEW_MODES
{
    MAP_VIEW_MODE     = 0,
    DUNGEON_VIEW_MODE = 1,
}

public class ViewModeFlipper : MonoBehaviour
{
    //Instead of hiding certain objects by deactivating them, instead hide them from the camera!
    public static EDITOR_VIEW_MODES EDITOR_VIEW_MODE;
    private const string mapLayerName = "MAP_UI";
    private const string dngLayerName = "DNG_UI";
    public CanvasGroup dngCanvas;
    public CanvasGroup mapCanvas;
    

    private void Start()
    {
        SwitchViewMode(0);
    }

    public void SwitchViewMode(int groupIndex)
    {
        EDITOR_VIEW_MODE = (EDITOR_VIEW_MODES)groupIndex;
        ShowAllLayers();
        
        switch (EDITOR_VIEW_MODE)
        {
            case EDITOR_VIEW_MODES.MAP_VIEW_MODE:
                    ToggleLayer(dngLayerName);
                    dngCanvas.blocksRaycasts = false;
                    mapCanvas.blocksRaycasts = true;
                    break;
            
            case EDITOR_VIEW_MODES.DUNGEON_VIEW_MODE:
                    ToggleLayer(mapLayerName);
                    dngCanvas.blocksRaycasts = true;
                    mapCanvas.blocksRaycasts = false;
                break;
            
            default:
                break;
        }

        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
    
    //Turn on the bit using an OR operation
    private void ShowLayer(string layerName)
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
    }

    private void ShowAllLayers()
    {
        Camera.main.cullingMask = -1;
    }
    
    //Turn off the bit using an and operation with the compliment of the shifted int
    private void HideLayer(string layerName)
    {
        Camera.main.cullingMask &= -(1 << LayerMask.NameToLayer(layerName));
    }
    
    //Toggle the bit using a XOR operation
    private void ToggleLayer(string layerName)
    {
        Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer(layerName);
    }
}
