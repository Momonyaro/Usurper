using System.Collections.Generic;
using UnityEngine;

using RULESET.ITEMS;

namespace RULESET.ENTITIES
{

    public abstract class Entity
    {
        public string name = "N/A";
        public int x = 0, y = 0;
        public int level = 1;
        public int xp = 0;
        public int gold = 0;
        public int[] stats = {1, 1, 1, 1, 1, 1, 1};     //Strength, Dexterity, Agility, Endurance, Perception, Charisma, Intelligence
        List<Item> inventory = new List<Item>();
        // Effects later
        List<BodyPart> bodyParts = new List<BodyPart>();
    }

}