using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    /*private PlayerMovement playerMovement;
    private Animator anim;
    [SerializeField] private float shootCooldown;
    private float cooldownTimer = float.MaxValue;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && shootCooldown > cooldownTimer && playerMovement.canShoot()) 
           Shoot();

        cooldownTimer += Time.deltaTime;
    }

    private void Shoot()
    {
        anim.SetTrigger("shoot");
        cooldownTimer = 0;
    }*/
}
