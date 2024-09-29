using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] public string nombreEscenaAJugar;
    
    public void ChangeScene()
    {
        SceneManager.LoadScene(nombreEscenaAJugar);
    }
}
