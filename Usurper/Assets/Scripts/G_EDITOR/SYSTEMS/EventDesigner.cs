using RULESET.MANAGERS;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = RULESET.MANAGERS.Event;
using System;
using UnityEngine.UIElements;
using System.Data;

namespace EDITOR.SYSTEMS
{
    public class EventDesigner : MonoBehaviour
    {
        public EventDesignerScriptable[] conditions;
        public EventDesignerScriptable[] actions;
        public EventDesignerScriptable[] constants;

        public Event currentEvent;
        private string currentKey;
        public List<EventTreeObj> eventTree;

        public Transform eventListParent;
        public GameObject eventListPrefab;

        public Transform eventLineParent;
        public GameObject eventLinePrefab;
        private Transform currentLineTransform;

        public GameObject eventObjDropPrefab;
        public GameObject eventObjFieldPrefab;
        public GameObject eventNewLineBtn;

        // We need a dropdown prefab and an inputfield to make up the UI. The ui elements are placed when the instruction is parsed.

        public void SetActiveEvent(string key)
        {
            if (!EventManager.events.TryGetValue(key, out currentEvent))
                Debug.LogError("Failed to set active event. No value found!");

            currentKey = key;

            BuildActiveEventUI();
        }

        public void BuildActiveEventUI()
        {
            if (currentEvent == null) return;
            WipeOldEventData();
            //When building active UI we don't need to use the child elements of the instructions since the data should be valid.

            for (int p = 0; p < currentEvent.eventData.Count; p++)
            {
                currentLineTransform = Instantiate(eventLinePrefab, eventLineParent).transform;

                BuildLineTreeFromCurrentEvent(p);
            }
            Instantiate(eventNewLineBtn, eventLineParent);
        }

        private void BuildLineTreeFromCurrentEvent(int line)
        {
            // int index = 0;

            // Now the only thing that should remain is constructing the UI. After this it should be working!
        }

        public void SubmitDataChange(int index, int line, EventDesignerScriptable.ArgumentType type, string newData)
        {
            ModifyInstruction(index, line, type, newData);
        }

        private void ModifyInstruction(int index, int line, EventDesignerScriptable.ArgumentType type, string newData)
        {
            int.TryParse(newData, out int result);

            switch (type)
            {
                case EventDesignerScriptable.ArgumentType.CONDITION:
                    ModifyTreeAtPosition(line, index, conditions[result]);
                    break;
                case EventDesignerScriptable.ArgumentType.ACTION:
                    ModifyTreeAtPosition(line, index, actions[result]);
                    break;
                case EventDesignerScriptable.ArgumentType.CONSTANT:
                    ModifyTreeAtPosition(line, index, constants[result]);
                    break;
                case EventDesignerScriptable.ArgumentType.FREE_VAR:
                    EventDesignerScriptable freeVar = ScriptableObject.CreateInstance<EventDesignerScriptable>();
                    freeVar.instructionCode = -1;
                    freeVar.instructionName = newData;
                    freeVar.prefabArgs = new EventDesignerScriptable.ArgumentType[0];
                    ModifyTreeAtPosition(line, index, freeVar);
                    break;
                case EventDesignerScriptable.ArgumentType.MEM_ADRR:
                    EventDesignerScriptable memAdrr = ScriptableObject.CreateInstance<EventDesignerScriptable>();
                    memAdrr.instructionCode = -2;
                    memAdrr.instructionName = newData;
                    memAdrr.prefabArgs = new EventDesignerScriptable.ArgumentType[0];
                    ModifyTreeAtPosition(line, index, memAdrr);
                    break;
            }
        }

        private EventTreeObj SearchTreeForPosition(int line, int index)
        {
            Stack<EventTreeObj> searchStack = new Stack<EventTreeObj>();
            searchStack.Push(eventTree[line]);

            while (searchStack.Count > 0)
            {
                EventTreeObj current = searchStack.Pop();
                if (current.line == line && current.index == index) return current;
                
                for (int i = current.children.Length; i > 0; i--)
                {
                    searchStack.Push(current.children[i]);
                }
            }

            return eventTree[line];
        }

        private void ModifyTreeAtPosition(int line, int index, EventDesignerScriptable scriptable)
        {
            EventTreeObj current = SearchTreeForPosition(line, index);

            current.instruction = scriptable;
            CreateTreeChildren(current);
            CompileEventFromTrees();
        }

        private void CreateTreeChildren(EventTreeObj treeObj)
        {
            treeObj.children = new EventTreeObj[treeObj.instruction.prefabArgs.Length];
            Stack<EventTreeObj> searchStack = new Stack<EventTreeObj>();
            searchStack.Push(treeObj);

            while (searchStack.Count > 0)
            {
                //Here for each object in the stack we check how many children it has and create tree objects for them before adding them to the stack as well
                EventTreeObj current = searchStack.Pop();

                for (int i = current.instruction.prefabArgs.Length; i > 0; i--)
                {
                    current.children[i] = ParseChildFromType(current.instruction.prefabArgs[i], current.line, current.index);
                    searchStack.Push(current.children[i]);
                }
            }
        }

        private EventTreeObj ParseChildFromType(EventDesignerScriptable.ArgumentType type, int line, int index)
        {
            EventTreeObj treeObj = new EventTreeObj
            {
                index = index,
                line = line
            };

            switch (type)
            {
                case EventDesignerScriptable.ArgumentType.CONDITION:
                    treeObj.instruction = conditions[0];
                    break;
                case EventDesignerScriptable.ArgumentType.ACTION:
                    treeObj.instruction = actions[0];
                    break;
                case EventDesignerScriptable.ArgumentType.CONSTANT:
                    treeObj.instruction = constants[0];
                    break;
                case EventDesignerScriptable.ArgumentType.FREE_VAR:
                    EventDesignerScriptable freeVar = ScriptableObject.CreateInstance<EventDesignerScriptable>();
                    freeVar.instructionCode = -1;
                    freeVar.instructionName = "0";
                    freeVar.prefabArgs = new EventDesignerScriptable.ArgumentType[0];
                    treeObj.instruction = freeVar;
                    break;
                case EventDesignerScriptable.ArgumentType.MEM_ADRR:
                    EventDesignerScriptable memAdrr = ScriptableObject.CreateInstance<EventDesignerScriptable>();
                    memAdrr.instructionCode = -2;
                    memAdrr.instructionName = "0";
                    memAdrr.prefabArgs = new EventDesignerScriptable.ArgumentType[0];
                    treeObj.instruction = memAdrr;
                    break;
            }

            return treeObj;
        }

        private void CompileEventFromTrees()
        {
            //Go through a recursive stack of the trees and for each tree compile the data to the event.
            Event compileResult = new Event
            {
                title = currentEvent.title
            };

            Stack<EventTreeObj> compileStack = new Stack<EventTreeObj>();

            for (int line = 0; line < eventTree.Count; line++)
            {
                compileStack.Clear();
                compileResult.eventData.Add(new List<string>());
                compileStack.Push(eventTree[line]);

                while (compileStack.Count > 0)
                {
                    EventTreeObj treeObj = compileStack.Pop();

                    for (int i = treeObj.children.Length; i > 0; i--)
                    {
                        compileStack.Push(treeObj.children[i]);
                    }

                    // Now we parse the instruction and basically place the instructionCode here or if it's -1 or -2 we pass the instructionName

                    if (treeObj.instruction.instructionCode == -1 || treeObj.instruction.instructionCode == -2)
                        compileResult.eventData[line].Add(treeObj.instruction.instructionName);
                    else
                        compileResult.eventData[line].Add(treeObj.instruction.instructionCode.ToString());
                }
            }

            if (EventManager.events.ContainsKey(currentKey))
            {
                EventManager.events[currentKey] = compileResult;
            }

            SetActiveEvent(currentKey);

            BuildActiveEventUI();
        }

        private void PopulateMemAdrrDropdown(Dropdown dropdown, string currentAdress)
        {
            Dropdown.OptionDataList dataList = new Dropdown.OptionDataList();
            int.TryParse(currentAdress, out int currentIndex);

            for (int i = 0; i < 64; i++)
            {
                dataList.options.Add(new Dropdown.OptionData($"MEM -> {i}"));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(dataList.options);
            dropdown.SetValueWithoutNotify(currentIndex);
        }

        private void PopulateInstructionDropdown(Dropdown dropdown, EventDesignerScriptable.ArgumentType argumentType, string currentAdress)
        {
            Dropdown.OptionDataList dataList = new Dropdown.OptionDataList();
            int.TryParse(currentAdress, out int currentIndex);
            int index = 0;

            if (argumentType == EventDesignerScriptable.ArgumentType.CONDITION)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    dataList.options.Add(new Dropdown.OptionData(conditions[i].instructionName));
                    if (conditions[i].instructionCode == currentIndex) index = i;
                }
            }
            else if (argumentType == EventDesignerScriptable.ArgumentType.ACTION)
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    dataList.options.Add(new Dropdown.OptionData(actions[i].instructionName));
                    if (actions[i].instructionCode == currentIndex) index = i;
                }
            }
            else
            {
                for (int i = 0; i < constants.Length; i++)
                {
                    dataList.options.Add(new Dropdown.OptionData(constants[i].instructionName));
                    if (constants[i].instructionCode == currentIndex) index = i;
                }
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(dataList.options);
            dropdown.SetValueWithoutNotify(index);
        }



        //Now that we effectivly parse instructions we need a way to create some stack to store the childArgs to correctly read in free_variables, NOPs and MEM_ADRR 

        //When pushing childArgs to the stack, do so in reverse since we want to access them in forward order and stacks are LIFO (Last In First Out)

        public void LoadEventList()
        {
            WipeOldEventList();
            Debug.Log(EventManager.events.Count);
            for (int i = 0; i < EventManager.events.Count; i++)
            {
                GameObject eventObject = Instantiate(eventListPrefab, eventListParent);
                ItemListItem listComponent = eventObject.GetComponent<ItemListItem>();

                listComponent.eventBtn = true;
                listComponent.eventKey = EventManager.events.ElementAt(i).Key;
                listComponent.designer = this;
                eventObject.transform.GetChild(0).GetComponent<Text>().text = $"{listComponent.eventKey} - {EventManager.events.ElementAt(i).Value.title}";
                eventObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        private void WipeOldEventList()
        {
            for (int i = 0; i < eventListParent.childCount; i++)
            {
                Destroy(eventListParent.GetChild(i).gameObject);
            }
        }

        private void WipeOldEventData()
        {
            for (int i = 0; i < eventLineParent.childCount; i++)
            {
                Destroy(eventLineParent.GetChild(i).gameObject);
            }
        }
    }

    public struct EventTreeObj
    {
        public int line;
        public int index;
        public EventDesignerScriptable instruction;
        public EventTreeObj[] children;

        public EventTreeObj(int line, int index, EventDesignerScriptable instruction)
        {
            this.line  = line;
            this.index = index;
            this.instruction = instruction;
            children = new EventTreeObj[instruction.prefabArgs.Length];
        }
    }
}
