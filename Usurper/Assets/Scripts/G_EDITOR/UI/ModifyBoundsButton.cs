using UnityEngine;
using EDITOR.MAP;

public class ModifyBoundsButton : MonoBehaviour
{
    public int value = 0;
    public MAP_BOUNDS_MODIFY_DIRECTIONS direction;

    public void ModifyBoundsOfActiveMap()
    {
        FindObjectOfType<EditorMap>().ResizeEditorMapBounds(value, direction);
    }
}