using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().Heal(gameObject);
        }
    }
}
