using Unity.VisualScripting;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] private float detectionRange;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    private PlayerMovement playerMovement;

    [SerializeField] private float chaseSpeed;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
        {
            Debug.LogError("Player object not found");
            return;
        }

        player = playerObj;
        playerMovement = playerObj.GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found");
        }
    }

    private void Update()
    {
        bool isPlayerInRange = Vector2.Distance(transform.position, player.transform.position) <= detectionRange;

        bool isFacingPlayer = (player.transform.position.x > transform.position.x && playerMovement._isFacingRight) ||
                      (player.transform.position.x < transform.position.x && !playerMovement._isFacingRight);

        if (isPlayerInRange && isFacingPlayer)
        {
            ChaseMovement();
        }
    }

    void ChaseMovement()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        if ((directionToPlayer.x > 0 && !playerMovement._isFacingRight) || (directionToPlayer.x < 0 && playerMovement._isFacingRight))
            Flip();
        transform.position += new Vector3(directionToPlayer.x * chaseSpeed * Time.deltaTime, 0, 0);
    }
    void Flip()
    {
        playerMovement._isFacingRight = !playerMovement._isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(transform.position, player.transform.position);

        UnityEditor.Handles.Label(
            (transform.position + player.transform.position) / 2, 
            $"Distance: {Vector2.Distance(transform.position, player.transform.position):F2}"
        );
    }


}