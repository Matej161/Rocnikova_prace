using UnityEngine;

public class MeleeEnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private Transform player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("player not found");
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private bool PlayerInRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= attackRange;
    }
    public void TryAttack()
    {
        if (cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("meleeAttack");
        }
    }
    public void DamagePlayer()
    {
        if (PlayerInRange())
            player.GetComponent<Health>()?.TakeDamage(damage, transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
