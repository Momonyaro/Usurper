using UnityEngine;
using EDITOR.SYSTEMS;
using RULESET.MANAGERS;

public class CreatureListItem : MonoBehaviour 
{
    public int index = 0;

    public void AddNewToCurrent()
    {
        EntityManager.creatures.Add(new RULESET.ENTITIES.CreatureSpecies("New Creature", "Creature Description", Resources.Load<Sprite>("Sprites/spr_err")));
        FindObjectOfType<CreatureDesigner>().PopulateCreatureList();
    }

    public void SetSelected()
    {
        CreatureDesigner.selected = EntityManager.creatures[index];
        FindObjectOfType<CreatureDesigner>().DrawCreature();
    }

    public void RemoveCreature()
    {
        EntityManager.creatures.RemoveAt(index);
        FindObjectOfType<CreatureDesigner>().PopulateCreatureList();
    }
}