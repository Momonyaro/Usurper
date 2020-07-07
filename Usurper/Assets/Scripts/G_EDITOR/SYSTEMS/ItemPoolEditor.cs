using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RULESET.ITEMS;

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
		public static List<ItemGroup> itemGroups = new List<ItemGroup>();
		public static List<Item> itemDatabase = new List<Item>();

		public ITEM_POOL_EDITOR_MODE editorMode = ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW;
		public GameObject listContentParent;
		public GameObject itemListPrefab;

		

		private void Start()
		{
			itemGroups.Add(new ItemGroup());
			itemGroups[0].id = 0;
			itemGroups[0].groupName = "TestGroup";
			itemGroups[0].groupSprite = Resources.Load<Sprite>("Sprites/UI/spr_ui_trash");

			itemDatabase.Add(new Item());
			itemDatabase[0].name = "TestItem";

			ChangeEditorViewAndDraw(0);
		}

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
					for (int i = 0; i < itemGroups.Count; i++)
					{
						GameObject instance = Instantiate(itemListPrefab, listContentParent.transform);
						ItemListItem listItem = instance.GetComponent<ItemListItem>();
						listItem.index = i;
						listItem.relevantMode = editorMode;
						listItem.propertyEditor = propertyEditor;

						instance.transform.GetChild(0).GetComponent<Text>().text = itemGroups[i].groupName;
                		instance.transform.GetChild(1).GetComponent<Image>().sprite = itemGroups[i].groupSprite;
					}
				break;

				case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
					for (int i = 0; i < itemDatabase.Count; i++)
					{
						GameObject instance = Instantiate(itemListPrefab, listContentParent.transform);
						ItemListItem listItem = instance.GetComponent<ItemListItem>();
						listItem.index = i;
						listItem.relevantMode = editorMode;
						listItem.propertyEditor = propertyEditor;

						instance.transform.GetChild(0).GetComponent<Text>().text = itemDatabase[i].name;
						instance.transform.GetChild(1).GetComponent<Image>().sprite = itemGroups[itemDatabase[i].groupId].groupSprite;
					}
				break;
			}

			DrawSelected();
		}

		public void DrawSelected()
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
	}

	[System.Serializable]
	public class ItemGroup
	{
		public int id = 0;
		public string groupName = "New Group";
		public Sprite groupSprite = null;
	}
}