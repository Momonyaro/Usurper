using UnityEngine;
using UnityEngine.UI;
using RULESET.ENTITIES;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{
    public class CreatureAnatomyPanel : MonoBehaviour 
    {
        public static EditorBodyPart selected;
        private EditorBodyPart copy = null;
        public GameObject bodyPartPrefab;
        public GameObject anatomyEditorParent;
        //Also decide how to display the stat view!

        public void PopulateAnatomyView()
        {
            for (int i = 0; i < anatomyEditorParent.transform.childCount; i++)
            {
                Destroy(anatomyEditorParent.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < CreatureDesigner.selected.bodyParts.Count; i++)
            {
                //Here we need to instantiate them
                EditorBodyPart bodyPart = CreatureDesigner.selected.bodyParts[i];
                GameObject instance = Instantiate(bodyPartPrefab, anatomyEditorParent.transform);
                instance.GetComponent<AnatomyListItem>().index = i;
                instance.GetComponent<RectTransform>().localPosition = new Vector3(bodyPart.bodyPartRect.x, bodyPart.bodyPartRect.y, 0);
                instance.GetComponent<RectTransform>().sizeDelta = new Vector2(bodyPart.bodyPartRect.width, bodyPart.bodyPartRect.height);
                instance.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, bodyPart.angle);
                if (selected == bodyPart) instance.GetComponent<Image>().color = new Color32(95, 126, 164, 255);
            }
            //Basically loop through the editorbodyparts and place bodyPartPrefabs at their locations.
            //Also if we're gonna add a click-to-place thing then make sure to check if we're clicking on a existing part or just on a blank spot!
        }

        public void MoveSelectedVertical(float distance)
        {
            if (selected != null)
                selected.bodyPartRect.y += distance;
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void MoveSelectedHorizontal(float distance)
        {
            if (selected != null)
                selected.bodyPartRect.x += distance;
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void ModifySelectedWidth(float amount)
        {
            if (selected != null)
                selected.bodyPartRect.width += amount;
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void ModifySelectedHeight(float amount)
        {
            if (selected != null)
                selected.bodyPartRect.height += amount;
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void RotateSelected(float amount)
        {
            if (selected != null)
                selected.angle -= amount;
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void RemoveSelected()
        {
            if (selected == null) return;
            for (int i = 0; i < CreatureDesigner.selected.bodyParts.Count; i++)
            {
                if (CreatureDesigner.selected.bodyParts[i] == selected)
                {
                    CreatureDesigner.selected.bodyParts.RemoveAt(i);
                    break;
                }
            }
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void AddNewSelected()
        {
            CreatureDesigner.selected.bodyParts.Add(new EditorBodyPart(new BodyPart(), new Rect(0, 0, 50, 50), 0));
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

        public void CopySelected()
        {
            if (selected == null) return;
            
            copy = selected.Copy();
        }

        public void PasteCopy()
        {
            if (copy == null || CreatureDesigner.selected == null) return;
            CreatureDesigner.selected.bodyParts.Add(copy.Copy());
            FindObjectOfType<CreatureDesigner>().DrawCreature();
        }

    }
}