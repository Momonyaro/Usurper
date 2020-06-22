using UnityEngine;
using RENDERER.MAP;

public class ModifyBoundsButton : MonoBehaviour
{
    public int value = 0;
    public MAP_BOUNDS_MODIFY_DIRECTIONS direction;

    public void ModifyBoundsOfActiveMap()
    {
        FindObjectOfType<MapViewport>().ResizeEditorMapBounds(value, direction);
    }
}