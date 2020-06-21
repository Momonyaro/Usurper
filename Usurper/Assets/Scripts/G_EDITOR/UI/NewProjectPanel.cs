using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RENDERER.MAP;

public class NewProjectPanel : MonoBehaviour
{
    public InputField mapNameField;
    public InputField mapWidthField;
    public InputField mapHeightField;
    public Text mapInfoTextBox;

    private string mapName = "New Name";
    private int mapWidth = 1;
    private int mapHeight = 1;

    private void Start()
    {
        CreateMenu();
    }

    private void CreateMenu()
    {
        mapNameField.SetTextWithoutNotify(mapName);
        mapWidthField.SetTextWithoutNotify(mapWidth.ToString());
        mapHeightField.SetTextWithoutNotify(mapHeight.ToString());
        CreateInfoTextDump();
    }

    public void SubmitNewMap()
    {
        FindObjectOfType<MapViewport>().loadedWorld.CreateNewWorld(mapName, mapWidth, mapHeight);
    }

    private void CreateInfoTextDump()
    {
        mapInfoTextBox.text = "Map Info:\n" + mapWidth + "x" + mapHeight + " chunks (" + mapWidth * mapHeight + ")\n" + mapWidth * 64 + "x" + mapHeight * 64 + " tiles (" + (mapWidth * 64) * (mapHeight * 64) +")";
        if (mapHeight > 10 || mapWidth > 10) mapInfoTextBox.text += "\n\n\n<color=red>This may take a while, go make some coffee or something...</color>";
    }

    public void SetMapName(string newName)
    {
        mapName = newName;
        CreateMenu();
    }

    public void SetMapWidth(string newWidth)
    {
        bool success = int.TryParse(newWidth, NumberStyles.Integer, null, out int intData);
        if (success) mapWidth = intData;
        CreateMenu();
    }

    public void SetMapHeight(string newHeight)
    {
        bool success = int.TryParse(newHeight, NumberStyles.Integer, null, out int intData);
        if (success) mapHeight = intData;
        CreateMenu();
    }
}
