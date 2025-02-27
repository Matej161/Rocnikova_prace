using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float idleDuration;
    private float idleTimer;
    private bool movingLeft;
    private Vector3 initScale;

    [Header("Chase Settings")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private LayerMask playerLayer;
    private Transform player;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D attackCollider;
    private float cooldownTimer;

    [Header("References")]
    [SerializeField] private Animator anim;

    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;

    private void Awake()
    {
        initScale = transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrolling;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (PlayerInDetectionRange())
                    currentState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
                Chase();
                if (!PlayerInDetectionRange())
                    currentState = EnemyState.Patrolling;
                else if (PlayerInAttackRange())
                    currentState = EnemyState.Attacking;
                break;

            case EnemyState.Attacking:
                Attack();
                if (!PlayerInAttackRange())
                    currentState = EnemyState.Chasing;
                break;
        }
    }

    private void Patrol()
    {
        anim.SetBool("moving", true);

        if (movingLeft)
        {
            if (transform.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (transform.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void Chase()
    {
        anim.SetBool("moving", true);

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += new Vector3(directionToPlayer.x * chaseSpeed * Time.deltaTime, 0, 0);

        // Flip enemy based on player position
        if (directionToPlayer.x > 0 && movingLeft || directionToPlayer.x < 0 && !movingLeft)
            Flip();
    }

    private void Attack()
    {
        anim.SetBool("moving", false);

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("meleeAttack");

            if (PlayerInAttackRange())
                DamagePlayer();
        }
    }

    private void MoveInDirection(int direction)
    {
        idleTimer = 0;
        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
        transform.position += new Vector3(direction * patrolSpeed * Time.deltaTime, 0, 0);
    }

    private void DirectionChange()
    {
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            Flip();
        }
    }

    private void Flip()
    {
        movingLeft = !movingLeft;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    private bool PlayerInDetectionRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private bool PlayerInAttackRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            attackCollider.bounds.center + transform.right * attackRange * transform.localScale.x * 0.5f,
            new Vector3(attackCollider.bounds.size.x * attackRange, attackCollider.bounds.size.y, attackCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void DamagePlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            attackCollider.bounds.center + transform.right * attackRange * transform.localScale.x * 0.5f,
            new Vector3(attackCollider.bounds.size.x * attackRange, attackCollider.bounds.size.y, attackCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            Health playerHealth = hit.transform.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            attackCollider.bounds.center + transform.right * attackRange * transform.localScale.x * 0.5f,
            new Vector3(attackCollider.bounds.size.x * attackRange, attackCollider.bounds.size.y, attackCollider.bounds.size.z));
    }
}