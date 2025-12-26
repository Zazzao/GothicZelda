using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 6.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        Debug.Log("SDADA");

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        Vector2 knockbackDir = (other.transform.position - transform.position).normalized;

        enemy.TakeDamage(damage, knockbackDir * knockbackForce);


    }




}
