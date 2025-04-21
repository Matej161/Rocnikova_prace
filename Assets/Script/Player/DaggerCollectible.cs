using UnityEngine;
using System.Collections;

public class DaggerCollectible : MonoBehaviour
{
    private PlayerDaggerInventory daggerInventory;

    [SerializeField] AudioClip collectSoundClip;
    [SerializeField] private float soundVolume;

    void Awake()
    {
        daggerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDaggerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            daggerInventory.AddDagger();
            SoundFXManager.Instance.PlaySoundFXClip(collectSoundClip, transform, soundVolume);

            Destroy(gameObject);
        }
    }
}