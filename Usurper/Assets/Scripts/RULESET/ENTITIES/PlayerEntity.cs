using UnityEngine;

namespace RULESET.ENTITIES
{

    public class PlayerEntity : Entity
    {
        public float xpScaler = 1;
        //Here we need a list of permanent strengths and weaknesses assigned when creating a character!
        //Perhaps we store them in saves as int but assign them to effects when loading the character?

        public int background;          // Another Dictionary Identifier!
    }

}