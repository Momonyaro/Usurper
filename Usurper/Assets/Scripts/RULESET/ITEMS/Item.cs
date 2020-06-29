using UnityEngine;

namespace RULESET.ITEMS
{

    public enum ITEM_CATEGORIES
    {
        ITEM_WEAPON,
        ITEM_AMMUNITION,
        ITEM_CLOTHING,
        ITEM_FOOD,
        ITEM_POTION,
        ITEM_QUEST,
        ITEM_MISC
    }

    public class Item
    {
        public string name = "N/A";
        public bool stackable = false;
        public int amount = 1;
        public int value = 0;
        public int weight = 0;     //In grams!
        public int x = 0;
        public int y = 0;
        public ITEM_CATEGORIES itemCatagory = ITEM_CATEGORIES.ITEM_MISC;
        //List of effects on the item here!!
    }

}