using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private int collisionDamage = 1;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) { 

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.ChangeHealth(-collisionDamage);
        }
    }


}
