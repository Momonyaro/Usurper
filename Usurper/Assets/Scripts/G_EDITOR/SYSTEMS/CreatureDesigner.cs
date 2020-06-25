using UnityEngine;
using System.Collections.Generic;
using RULESET.ENTITIES;

namespace EDITOR.SYSTEMS
{
    public class CreatureDesigner : MonoBehaviour
    {
        //basically just have a list of all creatures and display them
        //also store the latest selected and show it's properties on
        //the other panels!
    }

    public struct EditorCreature
    {
        string name;
        Sprite sprite;
        public int[] averageStats;
        // Add creature bonuses as well!
        public List<EditorBodyPart> bodyParts;
    }

    public struct EditorBodyPart
    {
        public BodyPart containedBodyPart;
        public Rect bodyPartRect;
        public Vector2 rectPos;
    }
}