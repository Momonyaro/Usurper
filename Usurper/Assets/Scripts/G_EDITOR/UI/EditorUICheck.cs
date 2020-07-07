using UnityEngine;
using RULESET.MANAGERS;

public class EditorUICheck : MonoBehaviour 
{
    public GameObject creatureDesigner;
    public GameObject itemDesigner;

    private void Update() 
    {
        InputManager.inEditorUI = false;
        if (creatureDesigner.activeInHierarchy || itemDesigner.activeInHierarchy)
        {
            InputManager.inEditorUI = true;
        }
    }
}