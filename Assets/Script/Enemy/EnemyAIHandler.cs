using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrolling")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float idleTime;

    [Header("Chasing")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private bool movingLeft = true;
    private bool isChasing = false;
    private Animator anim;
    private bool isIdle = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isIdle) return; // If idling, don't move

        bool playerInRange = Vector2.Distance(transform.position, player.position) <= detectionRange;

        if (playerInRange)
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
    }

    private void Patrol()
    {
        anim.SetBool("moving", true);
        float moveDirection = movingLeft ? -1 : 1;
        transform.position += new Vector3(moveDirection * patrolSpeed * Time.deltaTime, 0, 0);

        if (movingLeft && transform.position.x <= leftEdge.position.x + 0.1f)
        {
            StartCoroutine(IdleBeforeTurn());
        }
        else if (!movingLeft && transform.position.x >= rightEdge.position.x - 0.1f)
        {
            StartCoroutine(IdleBeforeTurn());
        }
    }

    private void ChasePlayer()
    {
        anim.SetBool("moving", true);
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.position += new Vector3(directionToPlayer * chaseSpeed * Time.deltaTime, 0, 0);

        if ((directionToPlayer > 0 && movingLeft) || (directionToPlayer < 0 && !movingLeft))
        {
            Flip();
        }
    }

    private IEnumerator IdleBeforeTurn()
    {
        isIdle = true;
        anim.SetBool("moving", false);
        yield return new WaitForSeconds(idleTime);
        movingLeft = !movingLeft;
        Flip();
        isIdle = false;
    }

    private void Flip()
    {
        movingLeft = !movingLeft;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
