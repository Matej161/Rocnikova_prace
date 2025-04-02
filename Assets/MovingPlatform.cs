using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float moveDistance = 5.0f;

    [Header("Player Interaction")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 startPosition;
    private Vector3 leftTarget;
    private Vector3 rightTarget;
    private Vector3 currentTarget;
    private bool movingRight = true;
    private bool movingUp= true;

    [SerializeField] private bool horizontalMovement = true;

    void Start()
    {
        startPosition = transform.position;

        leftTarget = startPosition - Vector3.right * (moveDistance / 2f);
        rightTarget = startPosition + Vector3.right * (moveDistance / 2f);

        currentTarget = rightTarget;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);

        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            // Switch smeru
            if (movingRight)
            {
                currentTarget = leftTarget;
                movingRight = false;
            }
            else
            {
                currentTarget = rightTarget;
                movingRight = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Check if the player landed on TOP of the platform
            // We check the collision normal. If the player is on top,
            // the normal pointing out from the platform's surface should be pointing upwards.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // contact.normal points outwards from the surface the collider hit
                // For a flat horizontal platform, the top surface normal is (0, 1) or Vector2.up
                if (contact.normal.y > 0.9f) // Use a threshold slightly less than 1 for robustness
                {
                    // Make the player a child of the platform
                    // This automatically makes the player move with the platform
                    collision.transform.SetParent(transform);
                    break; // Stop checking contacts once we confirm it's on top
                }
            }
        }
    }

    // Called when another Collider2D stops touching this platform's Collider2D
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the object leaving is the player
        if (collision.gameObject.CompareTag(playerTag))
        {
            // If the player was a child of this platform (meaning they were on top)
            // Set their parent back to null, detaching them from the platform
            // Check if the parent is actually this transform before unparenting
            // to avoid errors if the player somehow got reparented elsewhere while still colliding.
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }

    // Optional: Visualize the movement path in the Scene view
    private void OnDrawGizmosSelected()
    {
        // Ensure targets are calculated even if not playing (useful for setup)
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 left = start - Vector3.right * (moveDistance / 2f);
        Vector3 right = start + Vector3.right * (moveDistance / 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawWireSphere(left, 0.1f);
        Gizmos.DrawWireSphere(right, 0.1f);
        Gizmos.DrawWireSphere(start, 0.15f); // Mark the calculated center
    }
}