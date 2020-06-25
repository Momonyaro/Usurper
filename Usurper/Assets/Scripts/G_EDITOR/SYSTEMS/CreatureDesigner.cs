using UnityEngine;
using System.Collections.Generic;
using RULESET.ENTITIES;

namespace EDITOR.SYSTEMS
{
    public class CreatureDesigner : MonoBehaviour
    {
        public static EditorCreature selected;

        public List<EditorCreature> creatures = new List<EditorCreature>();

        public GameObject creaturePropertiesPanel;
        public GameObject creatureAnatomyPanel;

        private void Update()
        {
            if (selected != null && !creaturePropertiesPanel.activeInHierarchy)
            {
                creaturePropertiesPanel.SetActive(true);
                creatureAnatomyPanel.SetActive(true);
                DrawCreature(selected);
            }
        }

        public void DrawCreature(EditorCreature creature)
        {
            //Have a script on the properties panel to populate the fields correctly
            //Have a script on the anatomy editor to populate that correctly as well.
        }

        //basically just have a list of all creatures and display them
        //also store the latest selected and show it's properties on
        //the other panels!
    }

    public class EditorCreature
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
        public float angle;
    }
}