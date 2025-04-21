using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Color crystalColor; 
    public CrystalTracker crystalTracker;

    [SerializeField] AudioClip collectSoundClip;
    [SerializeField] private float soundVolume;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color opaqueColor = crystalColor;
            opaqueColor.a = 1f;
            crystalTracker.CollectCrystal(crystalColor);
            SoundFXManager.Instance.PlaySoundFXClip(collectSoundClip, transform, soundVolume);
            Destroy(gameObject); 
        }
    }
}