using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 30f;
    [SerializeField] protected float moveSpeed = 2f;

    [Header("Detection")]
    [SerializeField] protected float detectionRange = 6f;
    [SerializeField] protected Transform playerTarget;

    [Header("Combat")]
    [SerializeField] protected float attackRange = 1.2f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float attackWindup = 0.35f;
    [SerializeField] protected float recoveryTime = 0.5f;

    [Header("Knockback")]
    [SerializeField] protected float knockbackDuration = 0.15f;

    protected Rigidbody2D rb;

    protected float currentHealth;
    protected float nextAttackTime;
    protected float knockbackTimer;

    protected Vector2 facingDirection = Vector2.down;
    protected Vector2 knockbackVelocity;

    protected bool isDead;
    protected bool isKnockedBack;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    protected virtual void Awake(){
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        if (playerTarget == null){
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerTarget = player.transform;
        }
    }

    protected virtual void Update(){
        if (isDead) return;

        if (isKnockedBack){
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f){
                isKnockedBack = false;
                knockbackVelocity = Vector2.zero;
            }
        }
    }

    protected virtual void FixedUpdate(){
        if (isDead) return;

        if (isKnockedBack){
            rb.MovePosition(rb.position + knockbackVelocity * Time.fixedDeltaTime);
        }
    }

    protected bool HasTarget(){
        return playerTarget != null;
    }

    protected float DistanceToPlayer(){
        if (!HasTarget()) return float.PositiveInfinity;
        return Vector2.Distance(rb.position,playerTarget.position);
    }

    protected bool IsPlayerInDetectionRange(){
        return DistanceToPlayer() <= detectionRange;
    }

    protected bool IsPlayerInAttackRange(){
        return DistanceToPlayer() <= attackRange;
    }

    protected void FacePlayer(){
        if (!HasTarget()) return;

        Vector2 direction =
            (Vector2)playerTarget.position - rb.position;

        if (direction.sqrMagnitude > 0.001f)
            facingDirection = direction.normalized;
    }

    protected void MoveTowardsPlayer()
    {
        if (!HasTarget() || isKnockedBack)
            return;

        Vector2 direction =
            ((Vector2)playerTarget.position - rb.position)
            .normalized;

        facingDirection = direction;

        rb.MovePosition(
            rb.position +
            direction * moveSpeed * Time.fixedDeltaTime
        );
    }

    public virtual void TakeDamage(
        float damage,
        Vector2 knockbackDirection,
        float knockbackForce = 0f)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (knockbackForce > 0f)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            knockbackVelocity =
                knockbackDirection.normalized * knockbackForce;
        }

        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Stop movement and disable collisions.
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        foreach (Collider2D col in
                 GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        // Prototype behavior: remove the enemy immediately.
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
