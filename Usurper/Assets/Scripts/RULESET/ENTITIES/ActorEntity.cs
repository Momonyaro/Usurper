using UnityEngine;

namespace RULESET.ENTITIES
{

    public class ActorEntity : Entity
    {
        public string actorId = "00000";

        public int reputation;

        public int background;          // Dictionary Identifier!
        public int faction;             // Another Dictionary Identifier!
        public int occupation;          // Another Dictionary Identifier!
    }

}