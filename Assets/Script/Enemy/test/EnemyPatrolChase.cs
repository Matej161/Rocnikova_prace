using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolChase : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    public float speed;
    public float chaseSpeed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    [Header("Player Detection")]
    public float visionRange = 5f;
    public float attackRange = 1f;
    private Transform player;

    [Header("Obstacle Detection")]
    public float raycastDistance = 2f; 
    public LayerMask obstacleLayer; 

    private bool isChasing = false;
    private Health playerHealth;

    [SerializeField] bool YMovement;
    private float initialY;

    private void Awake()
    {
        initScale = enemy.localScale;
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        playerHealth = player.GetComponent<Health>();

        initialY = enemy.position.y;
    }

    private void OnDisable()
    {
        anim.SetBool("moving", false);
    }

    private void Update()
    {
        if (playerHealth.dead)
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

        if (distanceToPlayer <= visionRange && !IsObstacleInFront())
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (IsObstacleInFront())
        {
            anim.SetBool("moving", false);
        }
    }

    private void Patrol()
    {
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
        if (YMovement)
        {
            enemy.position = new Vector3(enemy.position.x, Mathf.Lerp(enemy.position.y, initialY, Time.deltaTime * 1), enemy.position.z);
        }
    }
    private void AttackPlayer()
    {
        anim.SetBool("moving", false);
        GetComponent<MeleeEnemyAttack>().TryAttack();
    }

    private bool IsObstacleInFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(enemy.position, transform.right, raycastDistance, obstacleLayer);

        return hit.collider != null;
    }

    private void DirectionChange()
    {
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            transform.position.y, enemy.position.z);
    }

    private void ChasePlayer()
    {
        if (Vector2.Distance(enemy.position, player.position) > attackRange)
        {
            //flip
            if (player.position.x > enemy.position.x)
                enemy.localScale = new Vector3(Mathf.Abs(initScale.x), initScale.y, initScale.z);
            else
                enemy.localScale = new Vector3(-Mathf.Abs(initScale.x), initScale.y, initScale.z);

            Vector2 direction = (player.position - enemy.position).normalized;
            enemy.position = Vector2.MoveTowards(new Vector2(enemy.position.x, transform.position.y), 
            new Vector2(player.position.x, transform.position.y), chaseSpeed * Time.deltaTime);

            if (!YMovement)
            {
                enemy.position = Vector2.MoveTowards(new Vector2(enemy.position.x, transform.position.y),
                new Vector2(player.position.x, transform.position.y), chaseSpeed * Time.deltaTime);
            }
            else if(YMovement)
            {
                enemy.position = Vector2.MoveTowards(new Vector2(enemy.position.x, transform.position.y),
                new Vector2(player.position.x, player.position.y), chaseSpeed * Time.deltaTime);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
