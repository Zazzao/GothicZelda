using UnityEngine;



public class RoomTransition : MonoBehaviour
{


    [SerializeField] BoxCollider2D col;
    [SerializeField] private float newMaxX;
    [SerializeField] private float newMinX;
    [SerializeField] private float newMaxY;
    [SerializeField] private float newMinY;


    [SerializeField] private Room fromRoom;
    [SerializeField] private Room toRoom;
    [SerializeField] private Vector2 playerOffset;




    [SerializeField] private Vector2 newPlayerOffset = Vector2.zero;

    void Awake(){
        col = GetComponent<BoxCollider2D>();
        if (col == null) Debug.Log("room transition obj missing box colider2d"); 
        col.isTrigger = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Debug.Log("On trigger hit");

        RoomTransitionManager.instance.StartTransition(fromRoom, toRoom, (Vector2)collision.gameObject.transform.position + playerOffset);

        //RoomTransitionManager.instance.Transition(fromRoom,toRoom,playerOffset);

    }


    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        Debug.Log("do room tranisition");

        //step 01: freeze player
        //playerMovement.IsFrozen = true;

        //step 02: make player invisible

        //step 03: pan camera to new location
        Camera.main.GetComponent<CameraFollow>().SetCameraBounds(newMinX, newMinY, newMaxX, newMaxY);

        //step 04: move player to new spot
        Transform p = playerMovement.gameObject.transform;
        p.position = new Vector3(p.transform.position.x + newPlayerOffset.x,
                                    p.transform.position.y + newPlayerOffset.y,
                                    p.transform.position.z);


        //step 05: make player visible
        //step 06: unfreeze player

    }
    //*/
}
