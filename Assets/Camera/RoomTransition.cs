using UnityEngine;



public class RoomTransition : MonoBehaviour
{


    [SerializeField] BoxCollider2D col;


    void Awake(){
        col = GetComponent<BoxCollider2D>();
        if (col == null) Debug.Log("room transition obj missing box colider2d"); 
        col.isTrigger = true;
    }

    
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        Debug.Log("do room tranisition");

        //step 01: freeze player
        playerMovement.IsFrozen = true;
        //step 02: make player invisible
        //step 03: pan camera to new location
        //step 04: move player to new spot
        //step 05: make player visible
        //step 06: unfreeze player

    }
}
