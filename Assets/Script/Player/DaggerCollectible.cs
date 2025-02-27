using UnityEngine;
using System.Collections;

public class DaggerCollectible : MonoBehaviour
{
    private PlayerDaggerInventory daggerInventory;

    void Awake()
    {
        daggerInventory = GetComponent<PlayerDaggerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("colliding with player");
            collision.GetComponent<PlayerDaggerInventory>().AddDagger();
            Destroy(gameObject);
        }
    }
}