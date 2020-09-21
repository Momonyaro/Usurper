using EDITOR.SYSTEMS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBlock : MonoBehaviour
{
    public int line;
    public int index;
    public int instructionCode;
    public EventDesignerScriptable.ArgumentType type;
    public EventDesigner designer;


    public void SubmitEventChangeField(string newData)
    {
        designer.SubmitDataChange(index, line, type, newData);
    }

    public void SubmitEventChangeDropdown(int index)
    {
        string data = "";

        if (type == EventDesignerScriptable.ArgumentType.CONDITION) data = designer.conditions[index].instructionCode.ToString();
        if (type == EventDesignerScriptable.ArgumentType.ACTION) data = designer.actions[index].instructionCode.ToString();
        if (type == EventDesignerScriptable.ArgumentType.CONSTANT) data = designer.constants[index].instructionCode.ToString();
        if (type == EventDesignerScriptable.ArgumentType.MEM_ADRR) data = index.ToString();

        designer.SubmitDataChange(this.index, line, type, data);
    }
}
