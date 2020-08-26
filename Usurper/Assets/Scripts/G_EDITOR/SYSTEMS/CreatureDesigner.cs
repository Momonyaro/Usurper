using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RULESET.ENTITIES;
using RULESET.MANAGERS;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{
    public class CreatureDesigner : MonoBehaviour
    {
        public static CreatureSpecies selected;
        private CreatureSpecies lastSelected;

        public GameObject creaturePropertiesPanel;
        public GameObject creatureAnatomyPanel;
        public GameObject creatureAnatomyPropertyPanel;
        public GameObject creatureListParent;
        public GameObject creatureListPrefab;

        private void Start() 
        {
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

            for (int i = 0; i < EntityManager.creatures.Count; i++)
            {
                GameObject instance = Instantiate(creatureListPrefab, creatureListParent.transform);
                if (selected == EntityManager.creatures[i]) { instance.GetComponent<Image>().color = new Color32(95, 126, 164, 255); }
                instance.GetComponent<CreatureListItem>().index = i;
                instance.transform.GetChild(0).GetComponent<Text>().text = EntityManager.creatures[i].name;
                instance.transform.GetChild(1).GetComponent<Image>().sprite = EntityManager.creatures[i].sprites[0];
            }
        }

        //basically just have a list of all creatures and display them
        //also store the latest selected and show it's properties on
        //the other panels!
    }
}