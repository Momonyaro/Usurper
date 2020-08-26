using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RULESET.ENTITIES;
using RENDERER.MAP;

namespace RULESET.MANAGERS
{
    public class EventManager : MonoBehaviour
    {
        public enum Conditions          //Range 000 - 063
        {
            DO                  = 0,
            IF                  = 1,
            ABOVE               = 2,
            BELOW               = 3,
        }

        public enum Actions             //Range 064 - 127
        {

            WARP_ACTOR          = 64,
            WARP_PLAYER         = 65,
            RENAME_ACTOR        = 66,
            GIVE_ACTOR_ITEM     = 67,
            GIVE_PLAYER_ITEM    = 68,
        }

        public enum Constants           //Range 128 - 255
        {
            PLAYER_HEALTH       = 128,
            PLAYER_MANA         = 129,
            PLAYER_LEVEL        = 130,
        }

        private int eventIndex = 0;
        private int lineIndex = 0;
        private Event currentEvent = new Event()
        {
            eventId = "ME325",
            eventData = new List<List<string>>() 
            { 
                new List<string>() { "0", "64", "MA191", "1", "1" },
                new List<string>() { "1", "2", "128", "-4", "65", "3", "3" }
            }
        };

        int constant = 0;
        int comparator = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) TestEvent();
        }

        public void TestEvent()
        {
            lineIndex = 0;
            for (int i = 0; i < currentEvent.eventData.Count; i++)
            {
                eventIndex = 0;
                if (currentEvent.eventData[lineIndex].Count <= eventIndex) return;    //Check if we're still in range
                bool success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out int result);    //Try to parse the next instruction
                if (!success) return;  //ERROR, we want to bail!
                ParseInstruction(result);
                lineIndex++;
            }
            GetComponent<TurnManager>().EditorEndTurn(Vector2Int.zero);
        }

        private bool ParseInstruction(int conditionIndex)
        {
            eventIndex++;   // Load the next instruction since most instructions call another instruction.
            if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
            bool success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out int result);    //Try to parse the next instruction
            //if (!success) return false;  //ERROR, we want to bail!
            switch (conditionIndex)
            {
                case (int)Conditions.DO:    // We expect the next thing in the currentEvent to be a action. Parse it as such
                        ParseInstruction(result);
                    return true;

                case (int)Conditions.IF:    // We expect the next thing in the currentEvent to be a condition. Parse it as such
                        if (ParseInstruction(result))   //This should be the Condition.
                        {
                            eventIndex++;   //Prepare to read the action
                            if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                            success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                            if (!success) return false; //Return if it fails
                            ParseInstruction(result);   //This call the Action
                            return true;
                        }
                    break;

                case (int)Conditions.ABOVE: // We expect a constant then a integer offset. compare them with a simple larger than operator.
                        constant = FetchConstant(result);
                    Debug.Log("Returned constant: " + constant + " from above statement");

                        eventIndex++;   //Prepare to read the int comparator
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails

                        comparator = result;

                    return (constant > comparator);

                case (int)Conditions.BELOW: // We expect a constant then a integer offset. compare them with a simple less than operator.
                        constant = FetchConstant(result);

                        eventIndex++;   //Prepare to read the int comparator
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails

                        comparator = result;

                    return (constant < comparator);


                case (int)Actions.WARP_ACTOR:
                        Entity relevantActor = EntityManager.FetchActorFromID(currentEvent.eventData[lineIndex][eventIndex]);
                        if (relevantActor == null) { Debug.Log("EVENT:ERROR | Could not find Actor with id : " + currentEvent.eventData[lineIndex][eventIndex]); return false; }

                        eventIndex++; 
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails
                        relevantActor.x = result;

                        eventIndex++;
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails
                        relevantActor.y = result;

                        return true;

                case (int)Actions.WARP_PLAYER:
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails
                        EntityManager.playerEntity.x = result;

                        eventIndex++;
                        if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
                        success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
                        if (!success) return false; //Return if it fails
                        EntityManager.playerEntity.y = result;

                    return true;

                case (int)Actions.RENAME_ACTOR:
                    break;

                case (int)Actions.GIVE_ACTOR_ITEM:
                    break;

                case (int)Actions.GIVE_PLAYER_ITEM:
                    break;
            }

            return false;
        }

        private int FetchConstant(int constantIndex)
        {
            switch(constantIndex)
            {
                case (int)Constants.PLAYER_HEALTH:
                    return EntityManager.playerEntity.health;
                case (int)Constants.PLAYER_MANA:
                    return EntityManager.playerEntity.mana;
                case (int)Constants.PLAYER_LEVEL:
                    return EntityManager.playerEntity.level;
            }
            return -1;
        }

        // DO               usage:  DO [ACTION]                                 always execute the [ACTION] (perhaps unneccesary in hindsight but it might be good for UI?)
        // IF               usage:  IF [CONDITION] [ACTION]                     if the [CONDITION] returns true, we execute the [ACTION]
        // ABOVE            usage:  ABOVE [CONSTANT] [comparator] [ACTION]      if the [CONSTANT] is above the [comparator] then we execute the [ACTION]
        // BELOW            usage:  BELOW [CONSTANT] [comparator] [ACTION]      if the [CONSTANT] is below the [comparator] then we execute the [ACTION]

        // WARP_ACTOR       usage:  WARP_ACTOR [ACTOR_ID] [posx] [posy]     warp the actor with the [ACTOR_ID] to a new position decided by [posx], [posy]
        // WARP_PLAYER      usage:  WARP_PLAYER [posx] [posy]               warp the player to a new position decided by [posx], [posy]
        // RENAME_ACTOR     usage:  RENAME_ACTOR [ACTOR_ID] [name]          set the displayed name of the actor with the [ACTOR_ID] to the new [name]
        // GIVE_ACTOR_ITEM  usage:  GIVE_ACTOR_ITEM [ACTOR_ID] [item name]  give the actor with the [ACTOR_ID] the stock item with name [item name]
        // GIVE_PLAYER_ITEM usage:  GIVE_PLAYER_ITEM [item name]            give the player the stock item with the name [item name]

    }

    public class Event
    {
        public string eventId;
        public List<List<string>> eventData;
    }
}
