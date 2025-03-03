using UnityEngine;
using System.Collections;

public class DaggerCollectible : MonoBehaviour
{
    private PlayerDaggerInventory daggerInventory;

    void Awake()
    {
        daggerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDaggerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            daggerInventory.AddDagger();
            Debug.Log("add dagger");
            Destroy(gameObject);
        }
    }
}