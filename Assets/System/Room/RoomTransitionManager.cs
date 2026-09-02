using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{

    public static RoomTransitionManager instance;

    //[SerializeField] private float panDuration = 0.4f;
    private bool isTransitioning = false;


    public Room currentRoom;

    private void Awake(){
        instance = this;
    }

    public void StartTransition(Room fromRoom, Room toRoom, Vector2 newPlayerPos) {
        if (isTransitioning) return;

        
        //player.IsFrozen = true;
        PlayerMotor.Instance.IsFrozen = true;
        PlayerMotor.Instance.GetComponent<SpriteRenderer>().enabled = false;
        PlayerMotor.Instance.transform.position = newPlayerPos;

        fromRoom.OnRoomExit();

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.SetCameraBounds(toRoom.minX, toRoom.maxX, toRoom.minY, toRoom.maxY); // to do: have this take room as input
        cam.SetPanTartget(newPlayerPos);
        cam.SetCameraState(CameraFollow.CameraState.Transition); 

        currentRoom = toRoom;

    }

    public void EndTransition() { 
        
        currentRoom.OnRoomEnter();

        PlayerMotor.Instance.IsFrozen = false;
        PlayerMotor.Instance.GetComponent<SpriteRenderer>().enabled = true;

        //set cam state to player follow
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.SetCameraState(CameraFollow.CameraState.Follow);
    }

    

}
