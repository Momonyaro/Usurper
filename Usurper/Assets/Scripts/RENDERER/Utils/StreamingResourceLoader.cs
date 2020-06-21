using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;


namespace RENDERER.UTILS
{

    public class StreamingResourceLoader : MonoBehaviour
    {
        public static bool finishedReading = false;

        //The idea is to load in maps, sprites, items & entities from external files in order to allow mods
        public void Init()
        {
            FetchSpritesForAtlas();
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(0, Atlas.SpriteAtlas.FetchSpriteByName("spr_void"), true, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(1, Atlas.SpriteAtlas.FetchSpriteByName("spr_grass_0"), false, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(2, Atlas.SpriteAtlas.FetchSpriteByName("spr_grass_1"), false, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(3, Atlas.SpriteAtlas.FetchSpriteByName("spr_grass_2"), false, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(4, Atlas.SpriteAtlas.FetchSpriteByName("spr_grass_1"), false, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(5, Atlas.SpriteAtlas.FetchSpriteByName("spr_grass_0"), false, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(6, Atlas.SpriteAtlas.FetchSpriteByName("spr_wall_0"), true, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(7, Atlas.SpriteAtlas.FetchSpriteByName("spr_boulder_0"), true, false));
            Atlas.TileAtlas.AddTileObjectToAtlas(new Atlas.TileObject(8, Atlas.SpriteAtlas.FetchSpriteByName("spr_str_lantern_0"), true, true));
            finishedReading = true;
        }


        //First test. Load spr_player and put it into a sprite atlas!
        private void FetchSpritesForAtlas()
        {
            List<Sprite> fetched = new List<Sprite>();
            string streamingStringPath = Application.streamingAssetsPath + "/Sprites";
            if (Directory.Exists(streamingStringPath))
            {
                string[] filePaths = Directory.GetFiles(streamingStringPath);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (filePaths[i].Contains("spr") && filePaths[i].Contains(".png") && !filePaths[i].Contains(".meta"))
                    {
                        byte[] pngData = File.ReadAllBytes(filePaths[i]);
                        // This will create a dependency on all sprites being 32x32... Perhaps look for another option later
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(pngData);
                        tex.filterMode = FilterMode.Point;
                        // Change texture into a sprite to pass onto Atlas
                        Sprite export = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32, 0);
                        export.name = filePaths[i].Replace(streamingStringPath + "/", "").Replace(".png", "");
                        if (export != null) { fetched.Add(export); Debug.Log(export.name); }
                    }
                }
            }
            Atlas.SpriteAtlas.AddSpriteArrayToAtlas(fetched.ToArray());
        }
    }

}