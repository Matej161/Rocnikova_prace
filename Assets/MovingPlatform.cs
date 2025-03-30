using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2.0f; // Speed of the platform
    [SerializeField] private float moveDistance = 5.0f; // Total distance the platform moves horizontally

    [Header("Player Interaction")]
    [SerializeField] private string playerTag = "Player"; // Tag of the player GameObject

    private Vector3 startPosition;
    private Vector3 leftTarget;
    private Vector3 rightTarget;
    private Vector3 currentTarget;
    private bool movingRight = true;

    // --- Initialization ---
    void Start()
    {
        // Store the initial position as the center of the movement
        startPosition = transform.position;

        // Calculate the left and right endpoints based on the moveDistance
        // We move distance/2 left and distance/2 right from the start position
        leftTarget = startPosition - Vector3.right * (moveDistance / 2f);
        rightTarget = startPosition + Vector3.right * (moveDistance / 2f);

        // Set the initial target based on the starting direction
        currentTarget = rightTarget;
    }

    // --- Movement Logic ---
    void Update()
    {
        // Calculate the movement step for this frame (frame-rate independent)
        float step = speed * Time.deltaTime;

        // Move the platform towards the current target position
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);

        // Check if the platform has reached the current target
        // Use a small tolerance to avoid floating point inaccuracies
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            // Switch direction
            if (movingRight)
            {
                currentTarget = leftTarget; // Target the left point
                movingRight = false;
            }
            else
            {
                currentTarget = rightTarget; // Target the right point
                movingRight = true;
            }
        }
    }

    // --- Player Attachment Logic ---

    // Called when another Collider2D enters contact with this platform's Collider2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player
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