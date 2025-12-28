using UnityEngine;




public class Enemy : MonoBehaviour{

    [SerializeField] private int maxHp = 3;
    private int hp;

    private Rigidbody2D rb;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6.0f;
    [SerializeField] private float knockbackDuration = 0.15f;

    EnemyAnimator anim;

    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity = Vector2.zero;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
        anim = GetComponent<EnemyAnimator>();
    }

    
    void FixedUpdate(){

        Vector2 moveDir;

        if (isKnockedBack){
            moveDir = knockbackVelocity;
        }
        else {
            moveDir = Vector2.zero;
        }

        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);

    }

    public void TakeDamage(int damageAmount, Vector2 sourcePosition){
        if (isKnockedBack) return;

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        

        hp -= damageAmount;
        if (hp <= 0) 
            Die();
        else
            ApplyKnockback(direction);
    }



    private void ApplyKnockback(Vector2 direction) {
        isKnockedBack = true;
        knockbackVelocity = direction * knockbackForce;
        anim.Play(ActorAnimator.ActorAnimation.Hit,ActorAnimator.FacingDirection.South,true,false);
        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback(){
        isKnockedBack = false;
        anim.Unlock();
        anim.Play(ActorAnimator.ActorAnimation.Idle, ActorAnimator.FacingDirection.South, false, false);

    }

    private void Die(){
        //Destroy(this.gameObject);
        Debug.Log("Enemy died");
        anim.Play(ActorAnimator.ActorAnimation.Dying,ActorAnimator.FacingDirection.South,true,true);   
        GetComponent<CapsuleCollider2D>().enabled = false;
    }






}
