using UnityEngine;
using EDITOR.SYSTEMS;

public class CreatureListItem : MonoBehaviour 
{
    public int index = 0;

    public void SetSelected()
    {
        CreatureDesigner.selected = CreatureDesigner.creatures[index];
        FindObjectOfType<CreatureDesigner>().DrawCreature();
    }
}