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

    [SerializeField] AudioClip swordSoundClip;
    [SerializeField] private float soundVolume;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.Mouse0) && playerMovement.IsGrounded())
        {
            StartAttack();
            SoundFXManager.Instance.PlaySoundFXClip(swordSoundClip, transform, soundVolume);
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            attackEndTime = Time.time + attackAnimationTime; 
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
        if (!playerMovement.enabled) 
            return;
        isAttacking = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);

        animator.SetTrigger("Attack");
        playerMovement.LockMovement(attackAnimationTime);
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
