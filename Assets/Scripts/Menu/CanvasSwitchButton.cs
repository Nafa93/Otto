using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitchButton : MonoBehaviour
{
    public Canvas newCanvas;
    public Canvas previousCanvas;


    public void Switch()
    {
        previousCanvas.gameObject.SetActive(false);
        // Activar el primer canvas y desactivar el segundo
        newCanvas.gameObject.SetActive(true);
    }
}
