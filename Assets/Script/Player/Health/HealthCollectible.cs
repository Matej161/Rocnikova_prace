using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    private SpriteRenderer spriteRend;
    private Health health;

    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;

    void Awake()
    {
        health = GetComponent<Health>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().Heal(gameObject); 
        }
    }
    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
            spriteRend.color = Color.gray;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}
