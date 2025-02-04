using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>(); //hashset nebere duplikaty

    private float attackAnimationTime = 0.7f; 
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
            isAttacking = true;
            attackEndTime = Time.time + attackAnimationTime; // When movement should resume
            StartAttack();
            nextAttackTime = Time.time + 1f / attackRate;
        }

        if (Time.time >= attackEndTime)
        {
            isAttacking = false;
        }

    }
    void StartAttack()
    {
        isAttacking = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);

        animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

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
            Enemy enemy = enemyCollider.GetComponent<Enemy>();  
            if (enemy != null && !damagedEnemies.Contains(enemy))  //jestli uz je enemy v hashsetu tak se if neprovede
            {
                enemy.TakeDamage(attackDamage);
                damagedEnemies.Add(enemy);
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
