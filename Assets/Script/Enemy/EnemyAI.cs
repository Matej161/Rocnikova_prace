using UnityEngine;

public class AdvancedEnemyController : MonoBehaviour
{
    // ========== CORE SETTINGS ==========
    [Header("Core Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointThreshold = 0.1f;

    // ========== DETECTION SETTINGS ==========
    [Header("Detection Settings")]
    [SerializeField] private Vector2 detectionSize = new Vector2(5f, 2f);
    [SerializeField] private Vector2 detectionOffset = new Vector2(0f, 0.5f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float visionCheckInterval = 0.25f;

    // ========== PHYSICS CHECKS ==========
    [Header("Physics Checks")]
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;

    // ========== COMPONENTS ==========
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private bool isDead = false;

    // ========== STATE MANAGEMENT ==========
    private enum AIState { Patrolling, Chasing }
    private AIState currentState = AIState.Patrolling;
    private int currentWaypointIndex = 0;
    private bool facingRight = true;
    private float lastVisionCheckTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (waypoints.Length == 0)
            Debug.LogError("No waypoints assigned to enemy!");
    }

    void Update()
    {
        if (isDead) return;

        HandleStateMachine();
        //UpdateAnimations();

        }
     
    void FixedUpdate()
    {
        if (isDead) return;

        if (Time.time - lastVisionCheckTime > visionCheckInterval)
        {
            CheckForPlayer();
            lastVisionCheckTime = Time.time;
        }
    }

    void HandleStateMachine()
    {
        if (currentState == AIState.Patrolling)
            PatrolMovement();
        else if (currentState == AIState.Chasing)
            ChaseMovement();
    }

    void PatrolMovement()
    {
        if (waypoints.Length == 0) return;

        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        // Flip check
        if ((moveDirection.x > 0 && !facingRight) || (moveDirection.x < 0 && facingRight))
            Flip();

        // Obstacle checks
        if (CheckForWall() || CheckForLedge())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(moveDirection.x * patrolSpeed, rb.velocity.y);

        // Waypoint update
        if (Vector2.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void ChaseMovement()
    {
        //Debug.Log("Chasing Player!");

        Vector2 playerDirection = (player.position - transform.position).normalized;

        // Flip check
        if ((playerDirection.x > 0 && !facingRight) || (playerDirection.x < 0 && facingRight))
            Flip();

        // Obstacle checks
        if (CheckForWall() || CheckForLedge())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(playerDirection.x * chaseSpeed, rb.velocity.y);
    }

    void CheckForPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapBox(
            (Vector2)transform.position + detectionOffset,
            detectionSize,
            0f,
            playerLayer
        );

        if (playerInRange != null)
        {
            Vector2 rayOrigin = transform.position;
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                directionToPlayer,
                distanceToPlayer,
                playerLayer
            );

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                currentState = AIState.Chasing;
            }
        }
        else
        {
            if (currentState == AIState.Chasing)
            {
                currentState = AIState.Patrolling;
            }
        }
    }

    bool CheckForWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position,
            facingRight ? Vector2.right : Vector2.left,
            wallCheckDistance,
            obstacleLayer
        );
        return hit.collider != null;
    }

    bool CheckForLedge()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            obstacleLayer
        );
        return hit.collider == null;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    /*void UpdateAnimations()
    {
        anim.SetBool("IsChasing", currentState == AIState.Chasing);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }*/

    public void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Disable physics
        anim.SetTrigger("Die");

        // Disable all behaviors
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        // Detection zone
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawCube((Vector2)transform.position + detectionOffset, detectionSize);

        // Vision ray
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Ground check
        Gizmos.color = Color.green;
        if (groundCheckPoint != null)
            Gizmos.DrawLine(groundCheckPoint.position,
                groundCheckPoint.position + Vector3.down * groundCheckDistance);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + detectionOffset, detectionSize);
    }
}