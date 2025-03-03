using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] daggers;

    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerDaggerInventory inventory;

    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        inventory = GetComponent<PlayerDaggerInventory>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        if (inventory.daggerCount > 0)
        {
            inventory.RemoveDagger();
            anim.SetTrigger("Shoot");
            cooldownTimer = 0;

            daggers[FindDagger()].transform.position = firePoint.position;
            daggers[FindDagger()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        } else
        {
            Debug.Log("no daggers");
        }
    }
    private int FindDagger()
    {
        for (int i = 0; i < daggers.Length; i++)
        {
            if (!daggers[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
