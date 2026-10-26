using UnityEngine;

public class PlayerAttackHitBox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 6.0f;

    private void OnTriggerEnter2D(Collider2D other){
        if (!other.CompareTag("Enemy")) return;

        Enemy_OLD enemy = other.GetComponent<Enemy_OLD>();
        if (enemy == null) return;

        enemy.TakeDamage(damage, this.transform.position);

    }




}
