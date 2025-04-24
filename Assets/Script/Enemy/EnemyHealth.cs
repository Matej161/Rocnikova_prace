using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] public float currentHealth { get; private set; }
    private Animator anim;
    public bool dead;

    private SpriteRenderer spriteRend;

    [SerializeField] CapsuleCollider2D hitCollider; 

    [SerializeField] public float fadeDelay = 5;

    [SerializeField] AudioClip damageSoundClip;
    [SerializeField] private float soundVolume;

    [SerializeField] AudioClip deathSoundClip;
    [SerializeField] private float deathVolume;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        SoundFXManager.Instance.PlaySoundFXClip(damageSoundClip, transform, soundVolume);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            if (!dead)
            {
                SoundFXManager.Instance.PlaySoundFXClip(damageSoundClip, transform, soundVolume);
                SoundFXManager.Instance.PlaySoundFXClip(deathSoundClip, transform, deathVolume);

                hitCollider.enabled = false;

                dead = true;
                anim.SetTrigger("die");

                GetComponent<MeleeEnemyAttack>().enabled = false;

                GetComponentInParent<EnemyPatrolChase>().speed = 0;

                GetComponentInParent<EnemyPatrolChase>().enabled = false;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                }


                StartCoroutine(DestroyAfterTime(fadeDelay));
            }
        }
    }
    private IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        Color color = spriteRend.color;

        for (float t = 0; t < 1; t += Time.deltaTime / 1.5f) 
        {
            spriteRend.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }

        Destroy(gameObject);
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
}