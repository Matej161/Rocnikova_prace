using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    [SerializeField] AudioClip backgroundMusicSoundClip;
    [SerializeField] private float soundVolume;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //SoundFXManager.Instance.PlaySoundFXClip(backgroundMusicSoundClip, transform, soundVolume);
    }
}