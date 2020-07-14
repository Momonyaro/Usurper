using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


namespace RENDERER.UTILS.Atlas
{
    public class TileAtlas : MonoBehaviour
    {
        public static List<TileObject> TileObjects = new List<TileObject>();
        public static List<TileObject> DngTileObjects = new List<TileObject>();

        public static void AddTileObjectToAtlas(TileObject tObject)
        {
            TileObjects.Add(tObject);
        }

        public static void AddTileObjectArrayToAtlas(TileObject[] tObjArr)
        {
            TileObjects.AddRange(tObjArr);
        }

        public static void SetTileObjectArrayToAtlas(TileObject[] tObjArr)
        {
            TileObjects.Clear();
            TileObjects.AddRange(tObjArr);
        }
        
        public static void AddTileObjectToDungeonAtlas(TileObject tObject)
        {
            DngTileObjects.Add(tObject);
        }

        public static void AddTileObjectArrayToDungeonAtlas(TileObject[] tObjArr)
        {
            DngTileObjects.AddRange(tObjArr);
        }

        public static void SetTileObjectArrayToDungeonAtlas(TileObject[] tObjArr)
        {
            DngTileObjects.Clear();
            DngTileObjects.AddRange(tObjArr);
        }

        public static TileObject FetchTileObjectById(int tObjId)
        {
            for (int i = 0; i < TileObjects.Count; i++)
            {
                if (TileObjects[i].id.Equals(tObjId))
                {
                    return TileObjects[i];
                }
            }
            Debug.Log("returning error tile.");
            return new TileObject(-1, SpriteAtlas.FetchSpriteByName("spr_err"), true, false, false);
        }
        
        public static TileObject FetchDungeonTileObjectById(int tObjId)
        {
            for (int i = 0; i < DngTileObjects.Count; i++)
            {
                if (DngTileObjects[i].id.Equals(tObjId))
                {
                    return DngTileObjects[i];
                }
            }
            Debug.Log("returning error tile.");
            return new TileObject(-1, SpriteAtlas.FetchSpriteByName("spr_err"), true, false, false);
        }

        public static void OverwriteTileAtIndex(int id, TileObject newTile)
        {
            for(int i = 0; i < TileObjects.Count; i++)
            {
                if (TileObjects[i].id == id)
                {
                    TileObjects[i] = newTile;
                    break;
                }
            }
        }

        public static void RemoveTileWithId(int id)
        {
            for (int i = 0; i < TileObjects.Count; i++)
            {
                if (TileObjects[i].id == id)
                {
                    TileObjects.RemoveAt(i);
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
