using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RENDERER.UTILS.Atlas
{
    public class SpriteAtlas : MonoBehaviour
    {
        //Holds all loaded sprites
        private static List<Sprite> sprites = new List<Sprite>();
        private const string spriteErrorFallbackName = "spr_err";

        public static void AddSpriteToAtlas(Sprite spr)
        {
            sprites.Add(spr);
        }

        public static void AddSpriteArrayToAtlas(Sprite[] sprArr)
        {
            sprites.AddRange(sprArr);
        }

        public static Sprite FetchSpriteByName(string sprName)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].name.Equals(sprName))
                {
                    return sprites[i];
                }
            }
            return null;
        }
    }
}

