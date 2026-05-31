using UnityEngine;

public class Spikes : MonoBehaviour
{

    [SerializeField] private int damage = 5;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.TakeDamage(damage,this.transform.position);
            
        }
    }
}
