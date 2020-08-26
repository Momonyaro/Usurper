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
        public int health = 0;
        public int mana = 0;
        public int level = 1;
        public int xp = 0;
        public int gold = 0;
        public int[] stats = {1, 1, 1, 1, 1, 1, 1};     //Strength, Dexterity, Agility, Endurance, Perception, Charisma, Intelligence
        public bool invulnerable = false;
        
        public int species;             // Dictionary identifier!
        public int spriteIndex = 0;
        public List<Item> inventory = new List<Item>();
        // Effects later
        public List<BodyPart> bodyParts = new List<BodyPart>();
    }

}