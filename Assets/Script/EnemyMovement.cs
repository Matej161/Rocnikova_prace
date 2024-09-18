using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject platform;

    void Start()
    {
        Destroy(platform);
    }

    void Update()
    {
        rb.velocity = Vector2.MoveTowards(rb.velocity, player.transform.position, moveSpeed);
    }
}