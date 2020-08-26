using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using RULESET.ENTITIES;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{
    public class CreaturePropertyPanel : MonoBehaviour 
    {
        public InputField nameField;
        public InputField idField;
        public InputField sprNameField;
        public InputField descField;
        public Image      sprPreview;
        //Also decide how to display the stat view!

        public void PopulateFieldsWithSelectedProperties()
        {
            nameField.SetTextWithoutNotify(CreatureDesigner.selected.name);
            idField.SetTextWithoutNotify(CreatureDesigner.selected.id.ToString());
            if (CreatureDesigner.selected.sprites == null) { sprNameField.SetTextWithoutNotify("spr_err"); } else { sprNameField.SetTextWithoutNotify(CreatureDesigner.selected.sprites[0].name); }
            descField.SetTextWithoutNotify(CreatureDesigner.selected.desc);
            sprPreview.sprite = CreatureDesigner.selected.sprites[0];
            FindObjectOfType<CreatureDesigner>().PopulateCreatureList();
        }

        public void SetSelectedName(string newName)
        {
            CreatureDesigner.selected.name = newName;
            PopulateFieldsWithSelectedProperties();
        }

        public void SetSelectedID(string newID)
        {
            bool success = int.TryParse(newID, out int result);
            if (success) CreatureDesigner.selected.id = result;
        }

        public void SetSelectedSprFromName(string newSprName)
        {
            CreatureDesigner.selected.sprites = new List<Sprite>() { SpriteAtlas.FetchSpriteByName(newSprName) };
            PopulateFieldsWithSelectedProperties();
        }

        public void SetSelectedDesc(string newDesc)
        {
            CreatureDesigner.selected.desc = newDesc;
            PopulateFieldsWithSelectedProperties();
        }

    }
}