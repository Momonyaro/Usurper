using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS.Atlas;

public class PointerImageGhost : MonoBehaviour
{
    public static TileObject selected;
    public Image ghostProjector;


    public static void SetSelected(TileObject newSelection)
    {
        selected = newSelection;
    }

    public static void ClearSelected()
    {
        selected.tile = null;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
        //transform.position = Camera.main.ScreenToWorldPoint(transform.position);

        DrawGhostofSelected();
    }

    private void DrawGhostofSelected()
    {
        if (selected.tile == null)
        {
            ghostProjector.gameObject.SetActive(false);
            return;
        }
        ghostProjector.gameObject.SetActive(true);
        ghostProjector.sprite = selected.tile.sprite;
    }
}
