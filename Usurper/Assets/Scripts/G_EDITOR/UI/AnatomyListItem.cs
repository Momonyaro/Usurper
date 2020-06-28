using UnityEngine;
using EDITOR.SYSTEMS;

public class AnatomyListItem : MonoBehaviour 
{
    public int index = 0;

    public void SetSelected()
    {
        CreatureAnatomyPanel.selected = CreatureDesigner.selected.bodyParts[index];
        FindObjectOfType<CreatureDesigner>().DrawCreature();
    }
}