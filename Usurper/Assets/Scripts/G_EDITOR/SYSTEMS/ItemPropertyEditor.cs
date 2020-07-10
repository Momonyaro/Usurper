using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RULESET.ITEMS;
using RENDERER.UTILS.Atlas;

namespace EDITOR.SYSTEMS
{

    public class ItemPropertyEditor : MonoBehaviour 
    {
        //Here we need a property panel for groups and for items.
        //Handle them in different methods so that there is no crossfire for what to draw.
        public GameObject selectedItemPanel;
        public GameObject selectedGroupPanel;

        public void DrawSelectedItemProperties()
        {
            selectedGroupPanel.SetActive(false);
            selectedItemPanel.SetActive(true);

            selectedItemPanel.transform.GetChild(0).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.name);
            selectedItemPanel.transform.GetChild(1).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.desc);
            selectedItemPanel.transform.GetChild(2).GetComponent<Toggle>().SetIsOnWithoutNotify(ItemPoolEditor.selectedItem.stackable);
            selectedItemPanel.transform.GetChild(3).GetComponent<InputField>().interactable = true;
            if (!ItemPoolEditor.selectedItem.stackable)
            {
                selectedItemPanel.transform.GetChild(3).GetComponent<InputField>().interactable = false;
            }
            selectedItemPanel.transform.GetChild(3).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.amount.ToString());
            selectedItemPanel.transform.GetChild(4).GetComponent<Image>().sprite = ItemPoolEditor.itemGroups[ItemPoolEditor.selectedItem.groupId].groupSprite;
            selectedItemPanel.transform.GetChild(5).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.weight.ToString());
            selectedItemPanel.transform.GetChild(6).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.value.ToString());
            selectedItemPanel.transform.GetChild(7).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.groupId.ToString());
            selectedItemPanel.transform.GetChild(8).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.damage.ToString());
            selectedItemPanel.transform.GetChild(9).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.armor.ToString());
            selectedItemPanel.transform.GetChild(10).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.range.ToString());
            selectedItemPanel.transform.GetChild(11).GetComponent<Toggle>().SetIsOnWithoutNotify(ItemPoolEditor.selectedItem.usesAmmo);
            selectedItemPanel.transform.GetChild(12).GetComponent<InputField>().interactable = true;
            if (!ItemPoolEditor.selectedItem.usesAmmo)
            {
                selectedItemPanel.transform.GetChild(12).GetComponent<InputField>().interactable = false;
            }
            selectedItemPanel.transform.GetChild(12).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedItem.ammoType.ToString());
        }

        public void DrawSelectedGroupProperties()
        {
            if (ItemPoolEditor.selectedGroup.groupSprite == null)
            {
                ItemPoolEditor.selectedGroup.groupSprite = SpriteAtlas.FetchSpriteByName("spr_err");
            }
            
            selectedGroupPanel.SetActive(true);
            selectedItemPanel.SetActive(false);

            selectedGroupPanel.transform.GetChild(0).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedGroup.groupName);
            selectedGroupPanel.transform.GetChild(1).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedGroup.groupSprite.name);
            selectedGroupPanel.transform.GetChild(2).GetComponent<InputField>().SetTextWithoutNotify(ItemPoolEditor.selectedGroup.id.ToString());
            selectedGroupPanel.transform.GetChild(3).GetComponent<Image>().sprite = ItemPoolEditor.selectedGroup.groupSprite;
        }

        public void HideBothPanels()
        {
            selectedGroupPanel.SetActive(false);
            selectedItemPanel.SetActive(false);
        }

        #region UI SETTERS

        


        public void SetGroupName(string newName)
        {
            ItemPoolEditor.selectedGroup.groupName = newName;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemName(string newName)
        {
            ItemPoolEditor.selectedItem.name = newName;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetGroupSprite(string spriteName)
        {
            ItemPoolEditor.selectedGroup.groupSprite = SpriteAtlas.FetchSpriteByName(spriteName);
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetGroupId(string newId)
        {
            bool success = int.TryParse(newId, out int parsed);
            if (success) ItemPoolEditor.selectedGroup.id = parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemDesc(string newDesc)
        {
            ItemPoolEditor.selectedItem.desc = newDesc;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemStackable(bool stackable)
        {
            ItemPoolEditor.selectedItem.stackable = stackable;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemAmount(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.amount = (short)parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemWeight(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.weight = parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }

        public void SetItemValue(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.value = (short)parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemGroup(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.groupId = parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemDamage(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.damage = (short)parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemArmor(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.armor = (short)parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemRange(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.range = (short)parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemUsesAmmo(bool usesAmmo)
        {
            ItemPoolEditor.selectedItem.usesAmmo = usesAmmo;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        public void SetItemAmmoType(string amount)
        {
            bool success = int.TryParse(amount, out int parsed);
            if (success) ItemPoolEditor.selectedItem.ammoType = parsed;
            FindObjectOfType<ItemPoolEditor>().DrawEditorList();
        }
        
        #endregion
    }

}