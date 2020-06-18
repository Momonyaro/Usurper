using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.UTILS.Atlas;

public class ListTileObjContainer : MonoBehaviour
{
    TileObject thisTileObj = new TileObject(0, SpriteAtlas.FetchSpriteByName("spr_err"), false, false);


    public InputField  sprNameField;
    public InputField  tileIdField;
    public Toggle      colliderToggle;
    public Toggle      lightSrcToggle;
    public Image       tileSprViewer;

    private void Init()
    {
        sprNameField.SetTextWithoutNotify(thisTileObj.tile.sprite.name);
        tileIdField.SetTextWithoutNotify(thisTileObj.id.ToString());
        colliderToggle.SetIsOnWithoutNotify(thisTileObj.collider);
        lightSrcToggle.SetIsOnWithoutNotify(thisTileObj.lightSource);
        tileSprViewer.sprite = thisTileObj.tile.sprite;
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
    }

    public void FetchIDFromInput(string input)
    {
        bool success = int.TryParse(input, NumberStyles.Integer, null, out int newId);
        if (success) thisTileObj.id = newId;
    }

    public void SetColliderFromToggle(bool collider)
    {
        thisTileObj.collider = collider;
    }

    public void SetLightSrcFromToggle(bool lightSrc)
    {
        thisTileObj.lightSource = lightSrc;
    }

    public void SetPreviewSpr(Sprite spr)
    {
        tileSprViewer.sprite = spr;
    }

    public void SetGhostToThis()
    {
        PointerImageGhost.SetSelected(thisTileObj);
    }
}
