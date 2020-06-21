using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.MAP;

public class BoundsInfoText : MonoBehaviour
{
    public Text boundsInfoText;

    int cachedWidth = 0;
    int cachedHeight = 0;

    // Update is called once per frame
    void Update()
    {
        if (cachedWidth != World.width || cachedHeight != World.height)
        {   
            boundsInfoText.text = "'" + World.worldName + "'\nWidth: " + World.width + " Height: " + World.height;
            cachedWidth = World.width;
            cachedHeight = World.height;   
        }   
    }
}
