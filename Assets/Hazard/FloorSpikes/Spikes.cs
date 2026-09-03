using UnityEngine;

public class Spikes : MonoBehaviour{

    [SerializeField] private int damage = 5;

    private void OnTriggerEnter2D(Collider2D collision){

        //DEV NOTE: THis logic ONLY hurts the player - should update to hit anything that "walks" onto it
        if (!collision.gameObject.CompareTag("Player")) return;
        PlayerMotor.Instance.TakeDamage(damage, this.transform.position);
            
    }
}
