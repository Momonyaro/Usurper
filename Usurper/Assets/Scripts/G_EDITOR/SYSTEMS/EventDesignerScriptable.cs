using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Event Scriptable", menuName = "Event Scriptable", order = 1)]
public class EventDesignerScriptable : ScriptableObject
{
    public string instructionName;
    public int instructionCode;
    //Store Prefab references to required arguments?
    public GameObject[] prefabArgs;
}
