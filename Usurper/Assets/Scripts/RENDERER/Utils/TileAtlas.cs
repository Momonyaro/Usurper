using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


namespace RENDERER.UTILS.Atlas
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

        public static void SetTileObjectArrayToAtlas(TileObject[] tObjArr)
        {
            tileObjects.Clear();
            tileObjects.AddRange(tObjArr);
            foreach(var tileObj in tileObjects)
            {
            }
        }

        public static TileObject FetchTileObjectByID(int tObjId)
        {
            for (int i = 0; i < tileObjects.Count; i++)
            {
                if (tileObjects[i].id.Equals(tObjId))
                {
                    return tileObjects[i];
                }
            }
            Debug.Log("returning error tile.");
            return new TileObject(-1, SpriteAtlas.FetchSpriteByName("spr_err"), true, false, false);
        }

        public static void OverwriteTileAtIndex(int id, TileObject newTile)
        {
            for(int i = 0; i < tileObjects.Count; i++)
            {
                if (tileObjects[i].id == id)
                {
                    tileObjects[i] = newTile;
                    break;
                }
            }
        }

        public static void RemoveTileWithID(int id)
        {
            for (int i = 0; i < tileObjects.Count; i++)
            {
                if (tileObjects[i].id == id)
                {
                    tileObjects.RemoveAt(i);
                    break;
                }
            }
        }

    }

    [System.Serializable]
    public struct TileObject
    {
        public int id;
        public Tile tile;
        public bool collider;
        public bool transparent;
        public bool halfHeight;
        public bool lightSource;

        public TileObject(int id, Sprite sprite, bool collider, bool transparent, bool lightSource)
        {
            this.id = id;
            this.collider = collider;
            this.halfHeight = false;
            this.transparent = transparent;
            this.lightSource = lightSource;
            tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
            if (sprite != null)
            {
                tile.sprite = sprite;
                tile.name = "tile_" + sprite.name;
            }
            tile.colliderType = collider ? Tile.ColliderType.Grid : Tile.ColliderType.None;
        }

        public TileObject(int id, string spriteName, bool collider, bool transparent, bool lightSource)
        {
            this.id = id;
            this.collider = collider;
            this.halfHeight = false;
            this.transparent = transparent;
            this.lightSource = lightSource;
            tile = new Tile();
            tile.sprite = SpriteAtlas.FetchSpriteByName(spriteName); //PLEASE WORK;
            if (tile.sprite == null) tile.sprite = Resources.Load<Sprite>("Sprites/spr_err");
            tile.name = "tile_" + spriteName;
            tile.colliderType = collider ? Tile.ColliderType.Grid : Tile.ColliderType.None;
        }

        public TileObject Copy()
        {
            return new TileObject(id, tile.sprite, collider, transparent, lightSource);
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
