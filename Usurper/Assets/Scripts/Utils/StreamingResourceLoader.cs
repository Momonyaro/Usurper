using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StreamingResourceLoader : MonoBehaviour
{
    //The idea is to load in maps, sprites, items & entities from external files in order to allow mods
    void Start()
    {
        FetchSpritesForAtlas();
        GetComponent<SpriteRenderer>().sprite = Atlas.SpriteAtlas.FetchSpriteByName("spr_player");
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
