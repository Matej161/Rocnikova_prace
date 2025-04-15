using UnityEngine;

public class FloatingCollectible : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatHeight = 0.07f; 
    [SerializeField] private float floatSpeed = 4f;    

    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}