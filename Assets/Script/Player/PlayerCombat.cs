using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;
    public float attackRange = 0.5f;

    public LayerMask enemyLayer;

    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    public bool isAttacking;

    [SerializeField] float attackCooldown = 0.5f;

    private HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>(); //hashset nebere duplikaty

    private float attackAnimationTime = 0.45f; 
    private float attackEndTime;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.Mouse0) && playerMovement.IsGrounded())
        {
            StartAttack();
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            attackEndTime = Time.time + attackAnimationTime; // When movement should resume
            nextAttackTime = Time.time + 1f / attackRate;
        }

        if (Time.time >= attackEndTime)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
         
    }
    void StartAttack()
    {
        //if (canReceiveInput)
        //{
            isAttacking = true;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);

            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
            //inputReceived = true;
            //canReceiveInput = false; 
        //}
    }

    /*public void InputManager ()
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }*/


    public void EnableDamage()
    {
        damagedEnemies.Clear(); 
    }

    public void DetectEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();  
            if (enemyHealth != null && !damagedEnemies.Contains(enemyHealth))  //jestli uz je enemy v hashsetu tak se if neprovede
            {
                enemyHealth.TakeDamage(attackDamage);
                damagedEnemies.Add(enemyHealth);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
