using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


namespace Atlas
{
  public class TileAtlas : MonoBehaviour
  {
    public static List<TileObject> tileObjects = new List<TileObject>();

    public static void AddTileObjectToAtlas(TileObject tObject)
        {
            tileObjects.Add(tObject);
        }

        public static void AddTileObjectArrayToAtlas(TileObject[] tObjArr)
        {
            tileObjects.AddRange(tObjArr);
        }

        public static TileObject FetchTileObjectByID(int tObjId)
        {
            for (int i = 0; i < tileObjects.Count; i++)
            {
                if (tileObjects[i].id.Equals(tObjId))
                {
                  Debug.Log("Fetched tileObject: ID=" + tileObjects[i].id + " COLLIDER:" + tileObjects[i].collider + " LIGHTSOURCE:" + tileObjects[i].lightSource);
                  return tileObjects[i];
                }
            }
            Debug.Log("returning error tile.");
            return new TileObject(-1, SpriteAtlas.FetchSpriteByName("spr_err"), true, false);
        }

  }

  [System.Serializable]
  public struct TileObject
  {
    public int id;
    public Tile tile;
    public bool collider;
    public bool lightSource;

    public TileObject(int id, Sprite sprite, bool collider, bool lightSource)
    {
      this.id = id;
      this.collider = collider;
      this.lightSource = lightSource;
      tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
      tile.sprite = sprite;
      tile.name = "tile_" + sprite.name;
      tile.colliderType = collider ? Tile.ColliderType.Grid : Tile.ColliderType.None;
    }

    public TileObject(int id, string spriteName, bool collider, bool lightSource)
    {
      this.id = id;
      this.collider = collider;
      this.lightSource = lightSource;
      tile = new Tile();
      tile.sprite = SpriteAtlas.FetchSpriteByName(spriteName); //PLEASE WORK;
      tile.name = "tile_" + spriteName;
      tile.colliderType = collider ? Tile.ColliderType.Grid : Tile.ColliderType.None;
    }

    public Tile GetTileCopy()
    {
      Tile newCopy = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
      newCopy.name = tile.name;
      newCopy.sprite = tile.sprite;
      newCopy.colliderType = tile.colliderType;
      newCopy.color = tile.color;
      newCopy.flags = tile.flags;
      return newCopy;
    }
  }
}
