using UnityEngine;

public class Skeleton : Enemy
{
    private enum SkeletonState
    {
        Idle,
        Chasing,
        Windup,
        Recovery
    }

    [Header("Skeleton Attack")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackHitRadius = 0.65f;
    [SerializeField] private float attackOffset = 0.7f;
    [SerializeField] private LayerMask playerLayer;

    private SkeletonState state = SkeletonState.Idle;
    private float stateTimer;

    protected override void Update()
    {
        base.Update();

        if (isDead || isKnockedBack)
            return;

        if (!HasTarget())
        {
            state = SkeletonState.Idle;
            return;
        }

        switch (state)
        {
            case SkeletonState.Idle:
                UpdateIdle();
                break;

            case SkeletonState.Chasing:
                UpdateChasing();
                break;

            case SkeletonState.Windup:
                UpdateWindup();
                break;

            case SkeletonState.Recovery:
                UpdateRecovery();
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isDead || isKnockedBack)
            return;

        if (state == SkeletonState.Chasing)
            MoveTowardsPlayer();
    }

    private void UpdateIdle()
    {
        if (IsPlayerInDetectionRange())
            state = SkeletonState.Chasing;
    }

    private void UpdateChasing()
    {
        FacePlayer();

        if (!IsPlayerInDetectionRange())
        {
            state = SkeletonState.Idle;
            return;
        }

        if (IsPlayerInAttackRange() &&
            Time.time >= nextAttackTime)
        {
            BeginAttack();
        }
    }

    private void BeginAttack()
    {
        state = SkeletonState.Windup;
        stateTimer = attackWindup;

        FacePlayer();

        // Reserve the next attack time.
        nextAttackTime = Time.time + attackCooldown;

        // TODO: Play the skeleton windup animation.
    }

    private void UpdateWindup()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            PerformAttack();

            state = SkeletonState.Recovery;
            stateTimer = recoveryTime;
        }
    }

    private void PerformAttack()
    {
        Vector2 attackPosition;

        if (attackOrigin != null)
        {
            attackPosition = attackOrigin.position;
        }
        else
        {
            attackPosition = rb.position +
                facingDirection * attackOffset;
        }

        Collider2D hit = Physics2D.OverlapCircle(
            attackPosition,
            attackHitRadius,
            playerLayer
        );

        if (hit == null)
            return;

        IDamageable damageable =
            hit.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
        }

        // TODO: Play the skeleton attack animation.
        // TODO: Add attack sound and hit feedback.
    }

    private void UpdateRecovery()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            if (IsPlayerInDetectionRange())
                state = SkeletonState.Chasing;
            else
                state = SkeletonState.Idle;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Vector2 attackPosition = attackOrigin != null
            ? (Vector2)attackOrigin.position
            : (Vector2)transform.position +
              facingDirection * attackOffset;

        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            attackPosition,
            attackHitRadius
        );
    }
}
