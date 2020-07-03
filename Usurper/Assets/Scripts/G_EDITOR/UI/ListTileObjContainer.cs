using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS.Atlas;

public class ListTileObjContainer : MonoBehaviour
{
    TileObject thisTileObj = new TileObject(0, SpriteAtlas.FetchSpriteByName("spr_err"), false, false, false);
    public int index = 0;

    public InputField  sprNameField;
    public InputField  tileIdField;
    public Toggle      colliderToggle;
    public Toggle      lightSrcToggle;
    public Toggle      transparencyToggle;
    public Image       tileSprViewer;

    private void Init()
    {
        if (transparencyToggle != null) sprNameField.SetTextWithoutNotify(thisTileObj.tile.sprite.name);
        if (transparencyToggle != null) tileIdField.SetTextWithoutNotify(thisTileObj.id.ToString());
        if (transparencyToggle != null) colliderToggle.SetIsOnWithoutNotify(thisTileObj.collider);
        if (transparencyToggle != null) lightSrcToggle.SetIsOnWithoutNotify(thisTileObj.lightSource);
        if (transparencyToggle != null) transparencyToggle.SetIsOnWithoutNotify(thisTileObj.transparent);
        if (transparencyToggle != null) tileSprViewer.sprite = thisTileObj.tile.sprite;
    }

    public void SetThisTileObj(TileObject newTileObj)
    {
        thisTileObj = newTileObj;
        Init();
    }

    public void FetchSprFromInput(string input)
    {
        thisTileObj.tile.sprite = SpriteAtlas.FetchSpriteByName(input);
        if (thisTileObj.tile.sprite == null) thisTileObj.tile.sprite = Resources.Load<Sprite>("Sprites/spr_err");
        thisTileObj.tile.name = "tile_" + input;
        SetPreviewSpr(thisTileObj.tile.sprite);
        TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }

    public void FetchIDFromInput(string input)
    {
        bool success = int.TryParse(input, NumberStyles.Integer, null, out int newId);
        if (success) 
        { 
            int oldId = thisTileObj.id;
            thisTileObj.id = newId; 
            TileAtlas.OverwriteTileAtIndex(oldId, thisTileObj);
            StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
        }
    }

    public void SetColliderFromToggle(bool collider)
    {
        thisTileObj.collider = collider;
        TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }

    public void SetLightSrcFromToggle(bool lightSrc)
    {
        thisTileObj.lightSource = lightSrc;
        TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }

    public void SetTransparentFromToggle(bool transparent)
    {
        thisTileObj.transparent = transparent;
        TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }

    public void SetPreviewSpr(Sprite spr)
    {
        tileSprViewer.sprite = spr;
    }

    public void SetGhostToThis()
    {
        PointerImageGhost.SetSelected(thisTileObj);
    }

    public void AddNewTileToAtlas()
    {
        TileAtlas.AddTileObjectToAtlas(new TileObject(TileAtlas.tileObjects.Count, SpriteAtlas.FetchSpriteByName("spr_err"), false, false, false));
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }

    public void RemoveThisTileFromAtlas()
    {
        TileAtlas.RemoveTileWithID(index);
        StartCoroutine(FindObjectOfType<TileAtlasEditor>().LoadTileAtlasIfReady());
    }
}
