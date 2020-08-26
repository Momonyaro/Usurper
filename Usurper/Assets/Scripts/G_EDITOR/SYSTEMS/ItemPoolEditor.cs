using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RULESET.ITEMS;
using RULESET.MANAGERS;

//Here we just want a list with items. Present that list and allow editing the properties of individual elements.
//Also create functionality for adding / removing items from the pool
// In this editor we want users to be able to create weapon / armor types, ammunition types.
// After exporting this, we can use the databases to have a standard for items.
// If we want to just put a weapon in a chest, we can just call the database and get the standard version

namespace EDITOR.SYSTEMS
{
	public enum ITEM_POOL_EDITOR_MODE
	{
		ITEM_GROUP_VIEW,
		ITEM_DATABASE_VIEW
	}

	public class ItemPoolEditor : MonoBehaviour
	{
		public static ItemGroup selectedGroup;
		public static Item 		selectedItem;

		public ITEM_POOL_EDITOR_MODE editorMode = ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW;
		public GameObject listContentParent;
		public GameObject itemListPrefab;

		//Create something to populate the editor, also make the item groups compile into a dropdown.
		public void DrawEditorList()
		{
			for (int i = 0; i < listContentParent.transform.childCount; i++)
			{
				Destroy(listContentParent.transform.GetChild(i).gameObject);
			}

			ItemPropertyEditor propertyEditor = FindObjectOfType<ItemPropertyEditor>();

			switch (editorMode)
			{
				case ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW:
					for (int i = 0; i < ItemManager.itemGroups.Count; i++)
					{
						GameObject instance = Instantiate(itemListPrefab, listContentParent.transform);
						ItemListItem listItem = instance.GetComponent<ItemListItem>();
						listItem.index = i;
						listItem.relevantMode = editorMode;
						listItem.propertyEditor = propertyEditor;

						instance.transform.GetChild(0).GetComponent<Text>().text = ItemManager.itemGroups[i].groupName;
                		instance.transform.GetChild(1).GetComponent<Image>().sprite = ItemManager.itemGroups[i].groupSprite;
					}
					break;

				case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
					for (int i = 0; i < ItemManager.itemDatabase.Count; i++)
					{
						GameObject instance = Instantiate(itemListPrefab, listContentParent.transform);
						ItemListItem listItem = instance.GetComponent<ItemListItem>();
						listItem.index = i;
						listItem.relevantMode = editorMode;
						listItem.propertyEditor = propertyEditor;

						instance.transform.GetChild(0).GetComponent<Text>().text = ItemManager.itemDatabase[i].name;
						instance.transform.GetChild(1).GetComponent<Image>().sprite = ItemManager.itemGroups[ItemManager.itemDatabase[i].groupId].groupSprite;
					}
					break;
			}

			DrawSelected();
		}

		private void DrawSelected()
		{
			//Hand the itemPropertyEditor the relevant selected item.
			ItemPropertyEditor propertyEditor = FindObjectOfType<ItemPropertyEditor>();

			switch (editorMode)
        	{
				case ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW:
					if (selectedGroup != null) propertyEditor.DrawSelectedGroupProperties();
				break;

				case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
					if (selectedItem != null) propertyEditor.DrawSelectedItemProperties();
				break;
        	}
		}

		public void ChangeEditorViewAndDraw(int newMode)
		{
			editorMode = (ITEM_POOL_EDITOR_MODE)newMode;
			FindObjectOfType<ItemPropertyEditor>().HideBothPanels();
			DrawEditorList();
		}

		public void AddNewToActiveList()
		{
			switch (editorMode)
			{
				case ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW:
					ItemManager.itemGroups.Add(new ItemGroup());
					break;

				case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
					ItemManager.itemDatabase.Add(new Item());
					break;
			}

			DrawEditorList();
		}
	}
}