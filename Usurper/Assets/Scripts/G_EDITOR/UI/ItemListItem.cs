using UnityEngine;
using EDITOR.SYSTEMS;

public class ItemListItem : MonoBehaviour 
{
    public int index = 0;
    public ITEM_POOL_EDITOR_MODE relevantMode = ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW;
    public ItemPropertyEditor propertyEditor;

    public void SetSelected()
    {
        switch (relevantMode)
        {
            case ITEM_POOL_EDITOR_MODE.ITEM_GROUP_VIEW:
                ItemPoolEditor.selectedGroup = ItemPoolEditor.itemGroups[index];
                propertyEditor.DrawSelectedGroupProperties();
            break;

            case ITEM_POOL_EDITOR_MODE.ITEM_DATABASE_VIEW:
                ItemPoolEditor.selectedItem = ItemPoolEditor.itemDatabase[index];
                propertyEditor.DrawSelectedItemProperties();
            break;
        }
    }
}