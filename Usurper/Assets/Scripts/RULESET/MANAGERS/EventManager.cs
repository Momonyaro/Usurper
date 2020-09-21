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
            AND                 = 4,
            OR                  = 5,
            NOR                 = 6,
            XOR                 = 7,
        }

        public enum Actions             //Range 064 - 127
        {

            WARP_ACTOR          = 64,
            WARP_PLAYER         = 65,
            RENAME_ACTOR        = 66,
            GIVE_ACTOR_ITEM     = 67,
            GIVE_PLAYER_ITEM    = 68,
            SET_ACTOR_THREAD    = 69,
            START_THREAD        = 70,
            START_QUEST         = 71,
            COMPLETE_QUEST      = 72,
            UPDATE_QUEST        = 73,
            WRITE_TO_MEM        = 74,
            FLUSH_MEM           = 75,

            NOP                 = 127
        }

        public enum Constants           //Range 128 - 255
        {
            PLAYER_HEALTH       = 128,
            PLAYER_MANA         = 129,
            PLAYER_LEVEL        = 130,
            ACTOR_HEALTH        = 131,  //Takes an actor ID as input!
            ACTOR_MANA          = 132,  //Takes an actor ID as input!
            ACTOR_LEVEL         = 133,  //Takes an actor ID as input!
            READ_MEM            = 135,  //Takes a int as input
        }

        private int eventIndex = 0;
        private int lineIndex = 0;

        private const int memBankSize = 64;

        public static Dictionary<string, Event> events = new Dictionary<string, Event>();
        public string[] memBank = new string[memBankSize];
        private Event currentEvent;

        int constant = 0;
        int comparator = 0;

        public int result = 0;
        public bool success = false;

        private void Start()
        {
            events.Add("ME301", 
            new Event()
            {
                title = "Test Event",
                eventData = new List<List<string>>()
                {
                    new List<string>() { "1", "4", "3", "128", "330", "2", "128", "-4", "65", "3", "3" },       //if warp_actor MA191 to 1,1 and player_health > -4 warp_player 3,3
                    new List<string>() { "0", "64", "MA191", "1", "1" },                                        //warp_actor 00000 to 5,4
                    new List<string>() { "0", "74", "5", "316" },                                               //write_to_mem index 5 "316"
                    new List<string>() { "1", "2", "135", "5", "128", "64", "00000", "9", "0" },                //if memBank[5] > player_health warp_actor 00000 8,1
                    //new List<string>() { "0", "75", "127" },                                                    //flush_mem nop
                }
            });

        }

        private void Update()
        {
        }

        public void TestEvent()
        {
            bool foundEvent = events.TryGetValue("ME301", out currentEvent);
            if (!foundEvent) return;
            lineIndex = 0;
            for (int i = 0; i < currentEvent.eventData.Count; i++)
            {
                eventIndex = -1;
                if (!TickProgramCounter()) break;
                ParseInstruction(result);
                lineIndex++;
            }
            GetComponent<TurnManager>().EditorEndTurn(Vector2Int.zero);
        }

        private bool ParseInstruction(int conditionIndex)
        {
            if (!TickProgramCounter()) return false;
            switch (conditionIndex)
            {
                case (int)Conditions.DO:    // We expect the next thing in the currentEvent to be a action. Parse it as such
                    Debug.Log("instruction: DO");
                    ParseInstruction(result);
                    return true;

                case (int)Conditions.IF:    // We expect the next thing in the currentEvent to be a condition. Parse it as such
                    Debug.Log("instruction: IF");
                    if (ParseInstruction(result))   //This should be the Condition.
                    {
                        if (!TickProgramCounter()) return false;
                        ParseInstruction(result);   //This call the Action
                        return true;
                    }
                    break;

                case (int)Conditions.ABOVE: // We expect a constant then a integer offset. compare them with a simple larger than operator.
                    Debug.Log("instruction: ABOVE");
                    constant = FetchConstant(result);

                    if (!TickProgramCounter()) return false;

                    comparator = result;

                    return (constant > comparator);

                case (int)Conditions.BELOW: // We expect a constant then a integer offset. compare them with a simple less than operator.
                    Debug.Log("instruction: BELOW");
                    constant = FetchConstant(result);

                    if (!TickProgramCounter()) return false;

                    comparator = result;

                    return (constant < comparator);

                case (int)Conditions.AND:
                    Debug.Log("instruction: AND");
                    if (ParseInstruction(result))   //This should be the Condition.
                    {
                        if (!TickProgramCounter()) return false;

                        if (ParseInstruction(result))   //This call the second condition
                        {
                            return true;
                        }
                    }

                    break;

                case (int)Conditions.OR:
                    Debug.Log("instruction: OR");
                    if (ParseInstruction(result))
                    {
                        return true;
                    }

                    if (!TickProgramCounter()) return false;

                    if (ParseInstruction(result))
                    {
                        return true;
                    }

                    break;

                case (int)Conditions.NOR:
                    Debug.Log("instruction: NOR");

                    if (!ParseInstruction(result))   //This should be the Condition.
                    {
                        if (!TickProgramCounter()) return false;

                        if (!ParseInstruction(result))   //This call the second condition
                        {
                            return true;
                        }
                    }

                    break;

                case (int)Conditions.XOR:
                    Debug.Log("instruction: XOR");
                    if (ParseInstruction(result))
                    {
                        if (!TickProgramCounter()) return false;

                        if (!ParseInstruction(result))   //This call the second condition
                        {
                            return true;
                        }
                        return false;
                    }

                    if (!TickProgramCounter()) return false;

                    if (!ParseInstruction(result))
                    {
                        if (!TickProgramCounter()) return false;

                        if (ParseInstruction(result))   //This call the second condition
                        {
                            if (!TickProgramCounter()) return false;

                            return ParseInstruction(result);   //Execute the action since both instructions returned different results
                        }
                        return false;
                    }

                    break;

                case (int)Actions.WARP_ACTOR:
                    Debug.Log("instruction: WARP_ACTOR");
                    Entity relevantActor = EntityManager.FetchActorFromID(currentEvent.eventData[lineIndex][eventIndex]);
                        if (relevantActor == null) { Debug.Log("EVENT:ERROR | Could not find Actor with id : " + currentEvent.eventData[lineIndex][eventIndex]); return false; }

                    if (!TickProgramCounter()) return false;
                        relevantActor.x = result;

                    if (!TickProgramCounter()) return false;
                        relevantActor.y = result;

                    return true;

                case (int)Actions.WARP_PLAYER:
                    Debug.Log("instruction: WARP_PLAYER");
                        EntityManager.playerEntity.x = result;

                    if (!TickProgramCounter()) return false;
                        EntityManager.playerEntity.y = result;

                    return true;

                case (int)Actions.RENAME_ACTOR:
                    break;

                case (int)Actions.GIVE_ACTOR_ITEM:
                    break;

                case (int)Actions.GIVE_PLAYER_ITEM:
                    break;

                case (int)Actions.WRITE_TO_MEM:
                    Debug.Log("instruction: WRITE TO MEMORY BANK");
                    comparator = result;
                    if (!TickProgramCounter()) return false;
                    memBank[comparator] = currentEvent.eventData[lineIndex][eventIndex];    //Should work since we're not converting it to an int in this case.
                    break;

                case (int)Actions.FLUSH_MEM:
                    Debug.Log("instruction: FLUSH MEMORY BANK");
                    memBank = new string[memBankSize];
                    break;

                case (int)Actions.NOP:
                    break;
            }

            return false;
        }

        private int FetchConstant(int constantIndex)
        {
            switch(constantIndex)
            {
                case (int)Constants.PLAYER_HEALTH:
                    Debug.Log("instruction: Constants.PLAYER_HEALTH");
                    return EntityManager.playerEntity.health;
                case (int)Constants.PLAYER_MANA:
                    Debug.Log("instruction: Constants.PLAYER_MANA");
                    return EntityManager.playerEntity.mana;
                case (int)Constants.PLAYER_LEVEL:
                    Debug.Log("instruction: Constants.PLAYER_LEVEL");
                    return EntityManager.playerEntity.level;

                case (int)Constants.READ_MEM:
                    if (!TickProgramCounter()) return -1;
                    Debug.Log("instruction: READ MEMORY BANK " + result);
                    if (!int.TryParse(memBank[result], out result)) return -2;
                    return result;
            }
            return -1;
        }

        private bool TickProgramCounter()
        {
            eventIndex++;
            if (currentEvent.eventData[lineIndex].Count <= eventIndex) return false;    //Check if we're still in range
            success = int.TryParse(currentEvent.eventData[lineIndex][eventIndex], out result);  //Try to parse the next instruction
            Debug.Log("result: " + result);
            return true;
        }

        // DO               usage:  DO [ACTION]                                 always execute the [ACTION] (perhaps unneccesary in hindsight but it might be good for UI?)
        // IF               usage:  IF [CONDITION] [ACTION]                     if the [CONDITION] returns true, we execute the [ACTION]
        // ABOVE            usage:  ABOVE [CONSTANT] [comparator] [ACTION]      if the [CONSTANT] is above the [comparator] then we execute the [ACTION]
        // BELOW            usage:  BELOW [CONSTANT] [comparator] [ACTION]      if the [CONSTANT] is below the [comparator] then we execute the [ACTION]
        // AND              usage:  AND [CONDITION] [CONDITION] [ACTION]        if both [CONSTANT]s are true, execute [ACTION]

        // WARP_ACTOR       usage:  WARP_ACTOR [ACTOR_ID] [posx] [posy]     warp the actor with the [ACTOR_ID] to a new position decided by [posx], [posy]
        // WARP_PLAYER      usage:  WARP_PLAYER [posx] [posy]               warp the player to a new position decided by [posx], [posy]
        // RENAME_ACTOR     usage:  RENAME_ACTOR [ACTOR_ID] [name]          set the displayed name of the actor with the [ACTOR_ID] to the new [name]
        // GIVE_ACTOR_ITEM  usage:  GIVE_ACTOR_ITEM [ACTOR_ID] [item name]  give the actor with the [ACTOR_ID] the stock item with name [item name]
        // GIVE_PLAYER_ITEM usage:  GIVE_PLAYER_ITEM [item name]            give the player the stock item with the name [item name]

    }

    public class Event
    {
        public string title;
        public List<List<string>> eventData;
    }
}
