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
        private EditorCreature lastSelected;

        public static List<EditorCreature> creatures = new List<EditorCreature>();

        public GameObject creaturePropertiesPanel;
        public GameObject creatureAnatomyPanel;
        public GameObject creatureAnatomyPropertyPanel;
        public GameObject creatureListParent;
        public GameObject creatureListPrefab;

        private void Start() 
        {
            creatures.Add(new EditorCreature("Humans test", "Human test description", SpriteAtlas.FetchSpriteByName("spr_human_commoner_0")));
            creatures.Add(new EditorCreature("Dreml test", "Dreml test descriprion", SpriteAtlas.FetchSpriteByName("spr_player")));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(  0,  90, 100, 150), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(  0, 205,  50,  60), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(  0, -15, 100,  45), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(-80,  60,  35, 150), -8));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect( 80,  60,  35, 150), 8));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(-75,  140,  50, 50), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect( 75,  140,  50, 50), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(-30,  -115,  35, 150), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect( 30,  -115,  35, 150), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(-45,  -205,  60, 30), 0));
            creatures[0].bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect( 45,  -205,  60, 30), 0));

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
            if (selected != lastSelected) CreatureAnatomyPanel.selected = null;
            //Have a script on the properties panel to populate the fields correctly
            creaturePropertiesPanel.GetComponent<CreaturePropertyPanel>().PopulateFieldsWithSelectedProperties();
            //Have a script on the anatomy editor to populate that correctly as well.
            creatureAnatomyPanel.GetComponent<CreatureAnatomyPanel>().PopulateAnatomyView();
            if (CreatureAnatomyPanel.selected == null)
            {
                creatureAnatomyPropertyPanel.SetActive(false);
            }
            else 
            {
                creatureAnatomyPropertyPanel.SetActive(true);
                creatureAnatomyPropertyPanel.GetComponent<AnatomyPropertyPanel>().PopulateProperties();
            }
            lastSelected = selected;
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
        public Sprite sprite; //Change to list to have a pool of sprites instead.
        public int[] averageStats;
        // Add creature bonuses as well!
        public List<EditorBodyPart> bodyParts;

        public EditorCreature(string name, string desc, Sprite sprite)
        {
            this.name = name;
            this.desc = desc;
            this.sprite = sprite;
            averageStats = new int[7] {1, 1, 1, 1, 1, 1, 1};
            bodyParts = new List<EditorBodyPart>();
        }
    }

    public class EditorBodyPart
    {
        public BodyPart containedBodyPart;
        public Rect bodyPartRect;
        public float angle;

        public EditorBodyPart(BodyPart containedBodyPart, Rect bodyPartRect, float angle)
        {
            this.containedBodyPart = containedBodyPart;
            this.bodyPartRect = bodyPartRect;
            this.angle = angle;
        }
    }
}