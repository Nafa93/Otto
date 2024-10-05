using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButtons : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip onClick;
    public AudioClip onHover;

    public void HoverSound()
    {
       audioSource.PlayOneShot(onHover);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(onClick);
    }
}
