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
        public void Init(int pixelsPerUnit = 32, int dngPixelsPerUnit = 64)
        {
            FetchSpritesForAtlas(pixelsPerUnit, dngPixelsPerUnit);
            StreamingResourceLoader.finishedReading = true;
        }


        //First test. Load spr_player and put it into a sprite atlas!
        private void FetchSpritesForAtlas(int pixelsPerUnit, int dngPixelsPerUnit)
        {
            List<Sprite> fetched = new List<Sprite>();
            List<Sprite> dngFetched = new List<Sprite>();
            string streamingStringPath = Application.streamingAssetsPath + "/Sprites";
            if (Directory.Exists(streamingStringPath))
            {
                string[] filePaths = Directory.GetFiles(streamingStringPath);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (filePaths[i].Contains("spr") && filePaths[i].Contains(".png") && !filePaths[i].Contains(".meta"))
                    {
                        byte[] pngData = File.ReadAllBytes(filePaths[i]);
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(pngData);
                        tex.filterMode = FilterMode.Point;
                        // Change texture into a sprite to pass onto Atlas
                        Sprite export = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0);
                        export.name = filePaths[i].Replace(streamingStringPath + "/", "").Replace(".png", "");
                        if (export != null) { fetched.Add(export); Debug.Log(export.name); }

                        continue;
                    }
                    if (filePaths[i].Contains("dng") && filePaths[i].Contains(".png") && !filePaths[i].Contains(".meta"))
                    {
                        byte[] pngData = File.ReadAllBytes(filePaths[i]);
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(pngData);
                        tex.filterMode = FilterMode.Point;
                        // Change texture into a sprite to pass onto Atlas
                        Sprite export = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), dngPixelsPerUnit, 0);
                        export.name = filePaths[i].Replace(streamingStringPath + "/", "").Replace(".png", "");
                        if (export != null) { dngFetched.Add(export); Debug.Log(export.name); }

                        continue;
                    }
                }
            }
            
            Atlas.SpriteAtlas.AddSpriteArrayToAtlas(fetched.ToArray());
            Atlas.SpriteAtlas.AddSpriteArrayToDungeonAtlas(dngFetched.ToArray());
        }
    }

}