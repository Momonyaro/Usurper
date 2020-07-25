using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using RENDERER.UTILS.Atlas;
using RULESET.MANAGERS;
using RENDERER.MAP;

public class PointerImageGhost : MonoBehaviour
{

    public enum SELECTED_ENTITY_TYPE
    {
        NONE,
        ENTITY_ADD,
        ENTITY_DEL,
        GATE_ADD,
        GATE_DEL
    }


    public static TileObject selected;
    public static PointerImageGhost instance;
    public static int placingEntityLayer = 0;
    public Image ghostProjector;


    private void Awake()
    {
        PointerImageGhost.instance = this;
    }

    public static void SetSelected(TileObject newSelection)
    {
        if (placingEntityLayer != 0) { Debug.Log("placingEntityLayer == " + placingEntityLayer); return; }
        selected = newSelection;
    }

    public void SetEntitySelected(int type)
    {
        placingEntityLayer = type;
    }

    public static void ClearSelected()
    {
        placingEntityLayer = (int)SELECTED_ENTITY_TYPE.NONE;
        selected.tile = null;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 transformedPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(transformedPos.x, transformedPos.y, 0);

        DrawGhostofSelected();
    }

    public static void WorkaroundCreateGate()
    {
        instance.CreateGateAtMousePos();
    }

    public void CreateGateAtMousePos()
    {
        //How do we create a gate at the correct position? We need the center pos to ground ourselves...
        Vector2Int viewCenter = FindObjectOfType<MapViewport>().centerPosOnMap;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
        Vector2Int localPos = viewCenter + roundedPos;  //Looks a bit funky? May need to be tweaked! -Sebastian
        int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);

        localPos.Clamp(new Vector2Int(viewCenter.x - halfWidth, viewCenter.y - halfWidth), new Vector2Int(viewCenter.x + halfWidth, viewCenter.y + halfWidth));

        Debug.Log("new gate localPos = " + localPos + (SELECTED_ENTITY_TYPE)placingEntityLayer);

        if (placingEntityLayer == (int)SELECTED_ENTITY_TYPE.GATE_ADD)
            GateManager.PlaceAtPosition(localPos.x, localPos.y);
        else if (placingEntityLayer == (int)SELECTED_ENTITY_TYPE.GATE_DEL)
            GateManager.RemoveAtPosition(localPos.x, localPos.y);
    }

    private void DrawGhostofSelected()
    {
        if (selected.tile == null || placingEntityLayer != 0)
        {
            ghostProjector.gameObject.SetActive(false);
            return;
        }
        ghostProjector.gameObject.SetActive(true);
        ghostProjector.sprite = selected.tile.sprite;
    }
}
