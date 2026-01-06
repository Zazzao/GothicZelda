using UnityEngine;



public class RoomTransition : MonoBehaviour{


    [SerializeField] BoxCollider2D col;
    [SerializeField] private float newMaxX;
    [SerializeField] private float newMinX;
    [SerializeField] private float newMaxY;
    [SerializeField] private float newMinY;


    [SerializeField] private Room fromRoom;
    [SerializeField] private Room toRoom;
    [SerializeField] private Vector2 playerOffset;


    //[SerializeField] private Vector2 newPlayerOffset = Vector2.zero;

    void Awake(){
        col = GetComponent<BoxCollider2D>();
        if (col == null) Debug.Log("room transition obj missing box colider2d"); 
        col.isTrigger = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        //Debug.Log("On trigger hit");

        RoomTransitionManager.instance.StartTransition(fromRoom, toRoom, (Vector2)collision.gameObject.transform.position + playerOffset);

        

    }


}
