using UnityEngine;

namespace RULESET.ENTITIES
{

    public class BodyPart
    {
        public string name = "Part Name";
        public bool countsForDamage = true;
        public int hitThreshold = 1;
        public int damageMultiplier = 100;
        public int canHoldType = 0;            // Dictionary identifier!
        ITEMS.Item holding = new ITEMS.Item();
        public int canEquipType = 0;           // Another Dictionary Identifier!
        ITEMS.Item equipped = new ITEMS.Item();
    }

}