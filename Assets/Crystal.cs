using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Color crystalColor; 
    public CrystalTracker crystalTracker; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color opaqueColor = crystalColor;
            opaqueColor.a = 1f;
            crystalTracker.CollectCrystal(crystalColor); 
            Destroy(gameObject); 
        }
    }
}