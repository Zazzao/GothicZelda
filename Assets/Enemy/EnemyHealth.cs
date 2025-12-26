using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int hp;

    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
    }

    public void TakeDamage(int damageAmount, Vector2 knockback) { 
        hp -= damageAmount;
        rb.AddForce(knockback,ForceMode2D.Impulse);

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die() {
        Destroy(this.gameObject);
    }

}
