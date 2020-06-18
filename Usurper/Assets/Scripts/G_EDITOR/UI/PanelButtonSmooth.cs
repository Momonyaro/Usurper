using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonSmooth : MonoBehaviour
{
    public bool panelOpen = false;
    public RectTransform panelTransform;

    public void TogglePanelSmooth(float smoothSpeed)
    {
        if (panelOpen)
        {
            StartCoroutine(MovePanel(panelTransform.position + new Vector3(250, 0, 0), smoothSpeed));
        }
        else 
        {
            StartCoroutine(MovePanel(panelTransform.position + new Vector3(-250, 0, 0), smoothSpeed));
        }
    }

    public IEnumerator MovePanel(Vector3 targetPosition, float smoothSpeed)
    {
        transform.GetChild(0).rotation = panelOpen ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 90);
        while (Vector3.Distance(panelTransform.position, targetPosition) > .1f)
        {
            panelTransform.position = Vector3.Slerp(panelTransform.position, targetPosition, smoothSpeed);
            yield return null;
        }

        panelOpen = !panelOpen;
        yield break;
    }
}
