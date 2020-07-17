using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EDITOR_VIEW_MODES
{
    MAP_VIEW_MODE     = 0,
    DUNGEON_VIEW_MODE = 1,
}

public class ViewModeFlipper : MonoBehaviour
{
    public static EDITOR_VIEW_MODES EDITOR_VIEW_MODE;
    [SerializeField]
    public List<ViewModeGroup> viewModeGroups;

    private void Start()
    {
        SwitchViewMode(0);
    }

    public void SwitchViewMode(int groupIndex)
    {
        EDITOR_VIEW_MODE = (EDITOR_VIEW_MODES)groupIndex;
        
        for (int i = 0; i < viewModeGroups.Count; i++)
        {
            bool active = (i == (int)EDITOR_VIEW_MODE) ? true : false;
            foreach (var gObject in viewModeGroups[i].groupMembers)
            {
                gObject.SetActive(active);
            }
        }
    }

    [System.Serializable]
    public class ViewModeGroup
    {
        public List<GameObject> groupMembers = new List<GameObject>();
    }
}
