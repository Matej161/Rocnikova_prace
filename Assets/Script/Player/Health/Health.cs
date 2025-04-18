using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] public float currentHealth { get; private set; }
    private Animator anim;
    [SerializeField] public bool dead;
    public Rigidbody2D rb;

    public GameObject healthCollectible;

    private PlayerMovement playerMovement;

    [SerializeField] private float XKnockbackPower;
    [SerializeField] private float YKnockbackPower;

    [SerializeField] AudioClip damageSoundClip;
    [SerializeField] private float soundVolume;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    public void TakeDamage(float _damage, Transform enemyPosition)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        SoundFXManager.Instance.PlaySoundFXClip(damageSoundClip, transform, soundVolume);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            playerMovement.LockMovement(0.2f);

            float knockbackDirection;
            if (transform.position.x >= enemyPosition.position.x)
            {
                knockbackDirection = 1f;
            }
            else
            {
                knockbackDirection = -1f;
            }

            rb.velocity = new Vector2(knockbackDirection * XKnockbackPower, YKnockbackPower);
        }
        else
        {
            if (!dead)
            {
                dead = true;
                anim.SetTrigger("die");

                if(GetComponent<PlayerMovement>() != null)
                    GetComponent<PlayerMovement>().enabled = false;

                rb.velocity = Vector2.zero;
                rb.gravityScale = 5f;

            }
        }
    }


    public void Heal(GameObject collectible)
    {
        if (currentHealth != 0 && currentHealth != startingHealth)
        {
            currentHealth++;
            Destroy(collectible);
        }
        else
        {
            Debug.Log("HP full");
        }
    }
}