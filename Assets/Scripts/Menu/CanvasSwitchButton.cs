using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitchButton : MonoBehaviour
{
    public Canvas newCanvas;
    public Canvas previousCanvas;
    public void Switch()
    {
        Time.timeScale = 1f;
        previousCanvas.gameObject.SetActive(false);
        if(newCanvas != null)
        newCanvas.gameObject.SetActive(true);
    }
}
