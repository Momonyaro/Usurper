using System.Collections.Generic;
using UnityEngine;

using RULESET.ITEMS;

namespace RULESET.ENTITIES
{

    public enum STATS    //Use these instead of integers for setting and getting stats from the stat array.
    {
        STAT_STRENGTH,
        STAT_DEXTERITY,
        STAT_AGILITY,
        STAT_ENURANCE,
        STAT_PERCEPTION,
        STAT_CHARISMA,
        STAT_INTELLIGENCE,
    }
    
    public abstract class Entity
    {
        public string name = "N/A";
        public int x = 0, y = 0;
        public int health = 0, maxHealth = 0;
        public int mana = 0, maxMana = 0;
        public int level = 1;
        public int xp = 0, xpNeeded = 0;
        public int gold = 0;
        public int[] stats = {1, 1, 1, 1, 1, 1, 1};     //Strength, Dexterity, Agility, Endurance, Perception, Charisma, Intelligence
        public bool invulnerable = false;
        
        public int species;             // Dictionary identifier!
        public int spriteIndex = 0;
        public int weightInGrams = 12000, maxWeightInGrams = 50000;
        public List<Item> inventory = new List<Item>();
        // Effects later
        public List<BodyPart> bodyParts = new List<BodyPart>();

        public int CalculateMaxHealth()
        {
            // Health = Mathf.FloorToInt((50 * level) * (1 + (endurance / 2)))
            return Mathf.FloorToInt((50 * level) * (1f + ((float)stats[(int)STATS.STAT_ENURANCE] / 2.0f)));
        }

        public float CalculateDodgeChance(int attackerAgility)
        {
            // Dodging = (1 - (weight / weightCap)) * agility / attackerAgility        //Looks like it'll work suprisingly well! :)
            return (1 - ((float)weightInGrams / (float)maxWeightInGrams)) * ((float)stats[(int)STATS.STAT_AGILITY] / (float)attackerAgility);
        }

        public int CalculateXPNeeded()
        {
            // xpNeeded = 25 + (level ^ (12 * .14)) * 3    //This gives a nice exponential curve. 
            // It might be a bit too steep but I think we can compensate with later enemy xp amounts.

            return Mathf.CeilToInt(25 + (Mathf.Pow(level, (1.68f))) * 3);
        }
    }

}