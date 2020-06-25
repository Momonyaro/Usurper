using UnityEngine;
using UnityEngine.UI;
using RULESET.ENTITIES;

namespace EDITOR.SYSTEMS
{
    public class CreaturePropertyPanel : MonoBehaviour 
    {
        public InputField nameField;
        public InputField sprNameField;
        public InputField descField;
        public Image      sprPreview;

        public EditorCreature currentlyEditing;

    }
}