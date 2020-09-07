using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RULESET.DUNGEONS
{
    public class DungeonCube : MonoBehaviour
    {
        //This need to hold all the 6 sprites for each side of the cube!
        public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        public void SetCubeSprite(Sprite sprite)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sprite = sprite;
            }
        }
    }
}
