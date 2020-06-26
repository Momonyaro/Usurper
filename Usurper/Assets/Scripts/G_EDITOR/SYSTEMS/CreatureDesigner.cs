using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RULESET.ENTITIES;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{
    public class CreatureDesigner : MonoBehaviour
    {
        public static EditorCreature selected;

        public static List<EditorCreature> creatures = new List<EditorCreature>();

        public GameObject creaturePropertiesPanel;
        public GameObject creatureAnatomyPanel;
        public GameObject creatureListParent;
        public GameObject creatureListPrefab;

        private void Start() 
        {
            creatures.Add(new EditorCreature());
            creatures.Add(new EditorCreature());

            //Populate creature list
            PopulateCreatureList();
        }

        private void Update()
        {
            if (selected != null && !creaturePropertiesPanel.activeInHierarchy)
            {
                creaturePropertiesPanel.SetActive(true);
                creatureAnatomyPanel.SetActive(true);
                DrawCreature();
            }
            
            if (selected == null && creaturePropertiesPanel.activeInHierarchy && creatureAnatomyPanel.activeInHierarchy)
            {
                creaturePropertiesPanel.SetActive(false);
                creatureAnatomyPanel.SetActive(false);
            }
        }

        public void DrawCreature()
        {
            //Have a script on the properties panel to populate the fields correctly
            creaturePropertiesPanel.GetComponent<CreaturePropertyPanel>().PopulateFieldsWithSelectedProperties();
            //Have a script on the anatomy editor to populate that correctly as well.
        }

        public void PopulateCreatureList()
        {
            for (int i = 0; i < creatureListParent.transform.childCount; i++)
            {
                Destroy(creatureListParent.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < creatures.Count; i++)
            {
                GameObject instance = Instantiate(creatureListPrefab, creatureListParent.transform);
                if (selected == creatures[i]) { instance.GetComponent<Image>().color = new Color32(95, 126, 164, 255); }
                instance.GetComponent<CreatureListItem>().index = i;
                instance.transform.GetChild(0).GetComponent<Text>().text = creatures[i].name;
                instance.transform.GetChild(1).GetComponent<Image>().sprite = creatures[i].sprite;
            }
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