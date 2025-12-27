using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private int collisionDamage = 2;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) { 

           
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.TakeDamage(collisionDamage, this.gameObject.transform.position);
        }
    }


}
