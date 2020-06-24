using UnityEngine;

namespace RULESET.ENTITIES
{

    public class BodyPart
    {
        public int hitThreshold;
        public int damageMultiplier;
        public int canHoldType;            // Dictionary identifier!
        ITEMS.Item holding;
        public int canEquipType;           // Another Dictionary Identifier!
        ITEMS.Item equipped;
    }

}