using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearEnemy : MonoBehaviour
{

    public float walkSpeed = 3f;

    Rigidbody2D rb;


    public enum WalkableDirection { Left, Right };

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value != WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if (value == WalkableDirection.Left) 
                {
                    walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(walkSpeed * Vector2.right.x, rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
