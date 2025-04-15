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
    //private bool movingUp= true;

    //[SerializeField] private bool horizontalMovement = true;

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
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.9f) 
                {
                    collision.transform.SetParent(transform);
                    break; 
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }
}