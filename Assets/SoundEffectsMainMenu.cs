using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsMainMenu : MonoBehaviour
{
    public AudioSource src;
    public AudioClip start;

    public void Button1()
    {
        src.clip = start;
        src.Play();
    }
}
