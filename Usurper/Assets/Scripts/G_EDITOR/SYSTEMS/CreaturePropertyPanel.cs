using UnityEngine;
using UnityEngine.UI;
using RULESET.ENTITIES;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{
    public class CreaturePropertyPanel : MonoBehaviour 
    {
        public InputField nameField;
        public InputField sprNameField;
        public InputField descField;
        public Image      sprPreview;
        //Also decide how to display the stat view!

        public void PopulateFieldsWithSelectedProperties()
        {
            nameField.SetTextWithoutNotify(CreatureDesigner.selected.name);
            if (CreatureDesigner.selected.sprite == null) { sprNameField.SetTextWithoutNotify("spr_err"); } else { sprNameField.SetTextWithoutNotify(CreatureDesigner.selected.sprite.name); }
            descField.SetTextWithoutNotify(CreatureDesigner.selected.desc);
            sprPreview.sprite = CreatureDesigner.selected.sprite;
            FindObjectOfType<CreatureDesigner>().PopulateCreatureList();
        }

        public void SetSelectedName(string newName)
        {
            CreatureDesigner.selected.name = newName;
            PopulateFieldsWithSelectedProperties();
            

        }

        public void SetSelectedSprFromName(string newSprName)
        {
            CreatureDesigner.selected.sprite = SpriteAtlas.FetchSpriteByName(newSprName);
            PopulateFieldsWithSelectedProperties();
        }

        public void SetSelectedDesc(string newDesc)
        {
            CreatureDesigner.selected.desc = newDesc;
            PopulateFieldsWithSelectedProperties();
        }

    }
}