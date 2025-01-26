using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;

    //movement
    private float horizontal;
    private bool isFacingRight = true;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float spritMultiplier = 2f;
    [SerializeField] private float currentSpeed;
     
    //jump
    private bool doubleJump;
    [SerializeField] private float jumpingPower;

    //dash
    private bool canDash = true;
    private bool isDashing;
    private float dashingCooldown = 1f;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingPower;

    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            animator.SetBool("IsDashing", true);
            return;
        }

        if (!isDashing)
        {
            animator.SetBool("IsDashing", false);
        }

        if (IsGrounded())
        {
            animator.ResetTrigger("jump");
            animator.SetBool("falling", false);
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            //spusti jump animaci
            animator.SetTrigger("jump");
        }

        if(rb.velocity.y < 0)
        {
            animator.SetBool("falling", true);
        }

        if (!IsGrounded())
        {
            tr.emitting = true;
        }
        else
        {
            tr.emitting = false;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetBool("falling", true);
            animator.ResetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = moveSpeed * spritMultiplier;
            animator.SetTrigger("IsSprinting");
            animator.ResetTrigger("NotSprinting");
        }
        else
        {
            currentSpeed = moveSpeed;
            animator.SetTrigger("NotSprinting");
            animator.ResetTrigger("IsSprinting");
        }

        Flip();

    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = new Vector2(horizontal * currentSpeed, rb.velocity.y);

        HandleLayers();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localsScale = transform.localScale;
            localsScale.x *= -1f;
            transform.localScale = localsScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, rb.velocity.y);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void HandleLayers()
    {
        if (!IsGrounded())
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (IsGrounded())
        {
            animator.SetLayerWeight(1, 0);
        }
    }

}