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

    [System.Serializable]
    public class Item
    {
        public string name = "New Item";
        public string desc = "Item Description";
        public bool stackable = false;
        public short amount = 1;
        public short value = 0;
        public int weight = 0;     //In grams!
        public int groupId = 0;
        public int x = 0;
        public int y = 0;

        public ITEM_CATEGORIES itemCategory = ITEM_CATEGORIES.ITEM_MISC;
        public DAMAGE_TYPES damageType = DAMAGE_TYPES.DMG_CRUSHING;
        public short damage = 0;
        public short armor = 0;
        public short range = 0;
        public bool  usesAmmo = false;
        public int   ammoType = 0;  //This will be set to the group of the ammo
    }

}