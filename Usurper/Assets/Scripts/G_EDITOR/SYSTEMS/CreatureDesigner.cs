using UnityEngine;
using System.Collections.Generic;
using RULESET.ENTITIES;

namespace EDITOR.SYSTEMS
{
    public class CreatureDesigner : MonoBehaviour
    {
        public static EditorCreature selected;

        public List<EditorCreature> playableCreatures = new List<EditorCreature>();
        public List<EditorCreature> nonPlayableCreatures = new List<EditorCreature>();

        //basically just have a list of all creatures and display them
        //also store the latest selected and show it's properties on
        //the other panels!
    }

    public struct EditorCreature
    {
        public string name;
        public string desc;
        public Sprite sprite;
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