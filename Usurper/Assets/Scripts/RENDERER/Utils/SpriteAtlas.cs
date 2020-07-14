using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RENDERER.UTILS.Atlas
{
    public class SpriteAtlas : MonoBehaviour
    {
        //Holds all loaded sprites
        private static List<Sprite> mapSprites = new List<Sprite>();
        private static List<Sprite> dngSprites = new List<Sprite>();
        private const string spriteErrorFallbackName = "spr_err";

        public static void AddSpriteToAtlas(Sprite spr)
        {
            mapSprites.Add(spr);
        }

        public static void AddSpriteArrayToAtlas(Sprite[] sprArr)
        {
            mapSprites.AddRange(sprArr);
        }
        
        public static void AddSpriteArrayToDungeonAtlas(Sprite[] sprArr)
        {
            dngSprites.AddRange(sprArr);
        }

        public static Sprite FetchSpriteByName(string sprName)
        {
            for (int i = 0; i < mapSprites.Count; i++)
            {
                if (mapSprites[i].name.Equals(sprName))
                {
                    return mapSprites[i];
                }
            }
            return Resources.Load<Sprite>("Sprites/spr_err");
        }
        
        public static Sprite FetchDungeonSpriteByName(string sprName)
        {
            for (int i = 0; i < dngSprites.Count; i++)
            {
                if (dngSprites[i].name.Equals(sprName))
                {
                    return dngSprites[i];
                }
            }
            return Resources.Load<Sprite>("Sprites/spr_err");
        }
    }
}

