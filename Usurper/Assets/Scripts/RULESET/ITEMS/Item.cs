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

    public enum DAMAGE_TYPES
    {
        DMG_PIERCING,
        DMG_SLASHING,
        DMG_CHOPPING,
        DMG_CRUSHING,

        DMG_BURNING,
        DMG_FREEZING,
        DMG_CORRODING,
        DMG_SHOCKING,
        DMG_ARCANE,
        DMG_WATER
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
        public DAMAGE_TYPES damageType = DAMAGE_TYPES.DMG_CRUSHING;
        //List of effects on the item here!!
        //How do we add ranged weapons that use ammo / mana?
        //Let's keep a reference to ammo type here then we simply search the player inventory for that ammo type and fire that.
    }

}