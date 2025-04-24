using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator animator;
    private PlayerCombat _playerCombat;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private float soundVolume;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _jumpingPower;

    private float _horizontalInput;
    public bool _isFacingRight = true;

    //private bool _isJumping = false;
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
        if (!PauseMenu.isPaused)
        {
            if (IsGrounded())
            {
                animator.SetBool("falling", false);
            }

            if (rb.velocity.y < 0)
            {
                animator.SetBool("falling", true);
            }


            HandleMovementInput();
            HandleJumpInput();

            if (IsGrounded())
            {
                animator.SetBool("isGrounded", true);
            }
            else
            {
                animator.SetBool("isGrounded", false);
            }
        }
    }
    private void FixedUpdate()
    {
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

    public bool CanMove => !_movementLocked && !_playerCombat.isAttacking;

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

            animator.SetLayerWeight(_airLayerIndex, 1f);
            SoundFXManager.Instance.PlaySoundFXClip(jumpClip, transform, soundVolume);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !_playerCombat.isAttacking) 
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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

    public void PlayFootstep()
    {
        if (footstepClips.Length > 0 && !footstepAudioSource.isPlaying && IsGrounded())
        {
            int index = Random.Range(0, footstepClips.Length);
            footstepAudioSource.clip = footstepClips[index];
            footstepAudioSource.volume = footstepVolume;
            footstepAudioSource.pitch = Random.Range(0.95f, 1.05f);
            footstepAudioSource.Play();
        }
    }
}