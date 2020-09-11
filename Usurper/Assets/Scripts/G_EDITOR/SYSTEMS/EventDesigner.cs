using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDesigner : MonoBehaviour
{
    public EventDesignerScriptable[] conditions;
    public EventDesignerScriptable[] actions;
    public EventDesignerScriptable[] constants;

    // I hope that this designer can read the event data, interpret it into the prefabs and allow modification and creation of the events.
}
