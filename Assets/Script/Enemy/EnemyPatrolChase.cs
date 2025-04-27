using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrolChase : MonoBehaviour
{
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    public float speed = 2f;
    public float chaseSpeed = 3f;
    private bool movingLeft;
    private Vector3 initScale;

    [SerializeField] private float idleDuration = 2f;
    private float idleTimer;

    public float visionRange = 5f;
    public float attackRange = 1f;
    public float raycastDistance = 2f;
    public LayerMask obstacleLayer; 

    [SerializeField] private Animator anim;
    [SerializeField] private Transform enemy; 
    private Transform player;
    private Health playerHealth;
    private Rigidbody2D rb;

    [SerializeField] private bool isFlying = false;
    private float initialY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initScale = enemy.localScale;
        initialY = enemy.position.y;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }

        rb.freezeRotation = true;

        if (isFlying)
        {
            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void OnDisable()
    {
        anim.SetBool("moving", false);
    }

    private void Update()
    {
        if (player == null || playerHealth == null || playerHealth.dead)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            anim.SetBool("moving", false);
            AttackPlayer();
            return;
        }

        if (distanceToPlayer <= visionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (isFlying)
        {
            float newY = Mathf.Lerp(enemy.position.y, initialY, Time.deltaTime * 2f);
            enemy.position = new Vector3(enemy.position.x, newY, enemy.position.z);
        }

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void MoveInDirection(int dir)
    {
        idleTimer = 0;
        anim.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);

        if (isFlying)
        {
            rb.velocity = new Vector2(dir * speed, 0f);
        }
        else
        {
            rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        }
    }

    private void DirectionChange()
    {
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;

        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void ChasePlayer()
    {
        int dir;

        if (player.position.x > enemy.position.x)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        anim.SetBool("moving", true);

        if (isFlying)
        {
            Vector2 directionToPlayer = (player.position - enemy.position).normalized;
            rb.velocity = directionToPlayer * chaseSpeed;
        }
        else
        {
            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
        }
    }

    private void AttackPlayer()
    {
        anim.SetBool("moving", false);
        if (isFlying)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        GetComponent<MeleeEnemyAttack>()?.TryAttack();
    }

    private bool IsObstacleInFront()
    {
        Vector2 direction = enemy.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(enemy.position, direction, raycastDistance, obstacleLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
