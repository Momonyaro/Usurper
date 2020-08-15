using UnityEngine;
using EDITOR.SYSTEMS;
using RULESET.ITEMS;
using RULESET.MANAGERS;

public class ItemListItem : MonoBehaviour 
{
    public int index = 0;
    public ITEM_POOL_EDITOR_MODE relevantMode = ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW;
    public bool addBtn = false;
    public ItemPropertyEditor propertyEditor;

    public void SetSelected()
    {
        switch (relevantMode)
        {
            case ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW:
                if (addBtn)
                {
                    ItemManager.itemGroups.Add(new ItemGroup());
                    break;
                }
                ItemPoolEditor.selectedGroup = ItemManager.itemGroups[index];
                propertyEditor.DrawSelectedGroupProperties();
            break;

            case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
                if (addBtn)
                {
                    ItemManager.itemDatabase.Add(new Item());
                    break;
                }
                ItemPoolEditor.selectedItem = ItemManager.itemDatabase[index];
                propertyEditor.DrawSelectedItemProperties();
            break;
        }
    }
}