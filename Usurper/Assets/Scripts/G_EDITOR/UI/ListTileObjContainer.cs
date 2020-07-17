using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS.Atlas;
using UnityEngine.WSA;

public class ListTileObjContainer : MonoBehaviour
{
    TileObject thisTileObj = new TileObject(0, SpriteAtlas.FetchSpriteByName("spr_err"), false, false, false);
    public int index = 0;
    public bool dungeonTile = false;
    public TileAtlasEditor parent;

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
        Debug.Log("parent for " + gameObject.name + " is: " + transform.parent.name);
    }

    public void SetThisTileObj(TileObject newTileObj)
    {
        thisTileObj = newTileObj;
        Init();
    }

    public void FetchSprFromInput(string input)
    {
        thisTileObj.tile.sprite = dungeonTile ? SpriteAtlas.FetchDungeonSpriteByName(input) : SpriteAtlas.FetchSpriteByName(input);
        if (thisTileObj.tile.sprite == null) thisTileObj.tile.sprite = Resources.Load<Sprite>("Sprites/spr_err");
        thisTileObj.tile.name = "tile_" + input;
        SetPreviewSpr(thisTileObj.tile.sprite);
        TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(parent.LoadTileAtlasIfReady());
    }

    public void FetchIDFromInput(string input)
    {
        bool success = int.TryParse(input, NumberStyles.Integer, null, out int newId);
        if (success) 
        { 
            int oldId = thisTileObj.id;
            thisTileObj.id = newId; 
            if (dungeonTile)
                TileAtlas.OverwriteDungeonTileAtIndex(oldId, thisTileObj);
            else
                TileAtlas.OverwriteTileAtIndex(oldId, thisTileObj);
            StartCoroutine(parent.LoadTileAtlasIfReady());
        }
    }

    public void SetColliderFromToggle(bool collider)
    {
        thisTileObj.collider = collider;
        if (dungeonTile)
            TileAtlas.OverwriteDungeonTileAtIndex(thisTileObj.id, thisTileObj);
        else
            TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(parent.LoadTileAtlasIfReady());
    }

    public void SetLightSrcFromToggle(bool lightSrc)
    {
        thisTileObj.lightSource = lightSrc;
        if (dungeonTile)
            TileAtlas.OverwriteDungeonTileAtIndex(thisTileObj.id, thisTileObj);
        else
            TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(parent.LoadTileAtlasIfReady());
    }

    public void SetTransparentFromToggle(bool transparent)
    {
        thisTileObj.transparent = transparent;
        if (dungeonTile)
            TileAtlas.OverwriteDungeonTileAtIndex(thisTileObj.id, thisTileObj);
        else
            TileAtlas.OverwriteTileAtIndex(thisTileObj.id, thisTileObj);
        StartCoroutine(parent.LoadTileAtlasIfReady());
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
        if (dungeonTile)
            TileAtlas.AddTileObjectToDungeonAtlas(new TileObject(TileAtlas.DngTileObjects.Count, SpriteAtlas.FetchSpriteByName("spr_err"), false, false, false));
        else
            TileAtlas.AddTileObjectToAtlas(new TileObject(TileAtlas.TileObjects.Count, SpriteAtlas.FetchSpriteByName("spr_err"), false, false, false));
        StartCoroutine(parent.LoadTileAtlasIfReady());
    }

    public void RemoveThisTileFromAtlas()
    {
        if (dungeonTile)
            TileAtlas.RemoveDungeonTileWithId(index);
        else
            TileAtlas.RemoveTileWithId(index);
        StartCoroutine(parent.LoadTileAtlasIfReady());
    }
}
