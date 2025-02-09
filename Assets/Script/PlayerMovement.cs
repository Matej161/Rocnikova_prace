using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator animator;
    private PlayerCombat _playerCombat;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _sprintMultiplier = 2f;
    [SerializeField] private float _jumpingPower;

    [Header("Dash")]
    [SerializeField] private float _dashingPower;
    [SerializeField] private float _dashingTime;
    private float _dashingCooldown = 1f;
    private bool _canDash = true;
    private bool _isDashing;

    private float _horizontalInput;
    private bool _isFacingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (_isDashing) return;

        if (_isDashing)
        {
            animator.SetBool("IsDashing", true);
            return;
        }

        if (!_isDashing)
        {
            animator.SetBool("IsDashing", false);
        }

        if (IsGrounded())
        {
            animator.ResetTrigger("jump");
            animator.SetBool("falling", false);
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("falling", true);
        }

        HandleMovementInput();
        HandleJumpInput();
        HandleDashInput();
        HandleSprintInput();
    }
    private void FixedUpdate()
    {
        if (_isDashing) return;

        if (_playerCombat.isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(_horizontalInput * _currentSpeed, rb.velocity.y);
        }

        HandleLayers();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    private void HandleMovementInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(_horizontalInput));

        if (!_playerCombat.isAttacking)
        {
            Flip();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded() && !_playerCombat.isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
            animator.SetTrigger("jump");
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !_playerCombat.isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetBool("falling", true);
            animator.ResetTrigger("jump");
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && !_playerCombat.isAttacking)
        {
            StartCoroutine(Dash());
        }
    }

    private void HandleSprintInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _currentSpeed = _moveSpeed * _sprintMultiplier;
            animator.SetTrigger("IsSprinting");
            animator.ResetTrigger("NotSprinting");
        }
        else
        {
            _currentSpeed = _moveSpeed;
            animator.SetTrigger("NotSprinting");
            animator.ResetTrigger("IsSprinting");
        }
    }

    private void Flip()
    {
        if (_isFacingRight && _horizontalInput < 0f || !_isFacingRight && _horizontalInput > 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 newScale = transform.localScale;
            newScale.x *= -1f;
            transform.localScale = newScale;
        }
    }
    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * _dashingPower, rb.velocity.y);
        tr.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
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