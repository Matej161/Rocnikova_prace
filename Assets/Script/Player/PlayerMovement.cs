using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator animator;
    private PlayerCombat _playerCombat;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _jumpingPower;

    [Header("Dash")]
    [SerializeField] private float _dashingPower;
    [SerializeField] private float _dashingTime;
    private float _dashingCooldown = 1f;
    private bool _canDash = true;
    public bool _isDashing;

    private float _horizontalInput;
    public bool _isFacingRight = true;

    private bool _isJumping = false;
    private int _airLayerIndex;

    private bool _movementLocked;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();

        _currentSpeed = _moveSpeed;

        _airLayerIndex = animator.GetLayerIndex("AirLayer");
        if (_airLayerIndex == -1)
        {
            Debug.LogError("Animator Layer 'Air' not found!");
        }
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
            //animator.ResetTrigger("jump");
            animator.SetBool("falling", false);
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("falling", true);
        }


        HandleMovementInput();
        HandleJumpInput();
        HandleDashInput();

        if (IsGrounded())
        {
            animator.SetBool("isGrounded", true);
        } else
        {
            animator.SetBool("isGrounded", false);
        }
    }
    private void FixedUpdate()
    {
        if (_isDashing) return;

        if (!CanMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(_horizontalInput * _currentSpeed, rb.velocity.y);
        }

        HandleLayers();
    }

    public bool CanMove => !_movementLocked && !_playerCombat.isAttacking && !_isDashing;

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
        bool grounded = IsGrounded();

        if (Input.GetButtonDown("Jump") && grounded && !_playerCombat.isAttacking)
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
            _isJumping = true;

            animator.SetLayerWeight(_airLayerIndex, 1f);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !_playerCombat.isAttacking) 
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            _isJumping = false; 
        }
    }

    private void HandleAirborneState()
    {
        bool grounded = IsGrounded();

        if (grounded)
        {
            animator.SetBool("falling", false); 
            _isJumping = false; 
            
            animator.SetLayerWeight(_airLayerIndex, Mathf.Lerp(animator.GetLayerWeight(_airLayerIndex), 0f, Time.deltaTime * 10f)); 
        }
        else 
        {
            animator.SetLayerWeight(_airLayerIndex, Mathf.Lerp(animator.GetLayerWeight(_airLayerIndex), 1f, Time.deltaTime * 10f)); 

            if (rb.velocity.y < 0 && !_isJumping) 
            {
                animator.SetBool("falling", true);
            }
            else if (rb.velocity.y >= 0)
            {
                animator.SetBool("falling", false);
            }
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && !_playerCombat.isAttacking)
        {
            StartCoroutine(Dash());
        }
    }

    public void Flip()
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
        animator.SetBool("IsDashing", true);
        _canDash = false;
        _isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * _dashingPower, 0);
        yield return new WaitForSeconds(_dashingTime);
        rb.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
        animator.SetBool("IsDashing", false);
        //animator.Play("PlayerRoll");
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

    public void LockMovement(float duration)
    {
        StartCoroutine(LockMovementRoutine(duration));
    }

    private IEnumerator LockMovementRoutine(float duration)
    {
        _movementLocked = true;
        yield return new WaitForSeconds(duration);
        _movementLocked = false;
    }


    public bool canAttack()
    {
        return _horizontalInput == 0 && IsGrounded();
    }

}