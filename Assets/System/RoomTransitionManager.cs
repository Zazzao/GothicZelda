using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{

    public static RoomTransitionManager instance;

    [SerializeField] private float panDuration = 0.4f;
    private bool isTransitioning = false;


    public Room currentRoom;

    private void Awake(){
        instance = this;
    }

    public void StartTransition(Room fromRoom, Room toRoom, Vector2 playerOffset) {
        if (isTransitioning) return;

        PlayerMovement player = PlayerMovement.instance;
        player.IsFrozen = true;
        player.gameObject.SetActive(false);
        player.transform.position = playerOffset;

        fromRoom.OnRoomExit();

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.SetCameraBounds(toRoom.minX, toRoom.maxX, toRoom.minY, toRoom.maxY); // to do: have this take room as input
        cam.SetPanTartget(playerOffset); // set pan target
       //Debug.Log ("pan Target" + playerOffset.x + ", " + playerOffset.y);
        cam.SetCameraState(CameraFollow.CameraState.Transition);  // set cam state to panning

        currentRoom = toRoom;


    }

    public void EndTransition() { 
        
        currentRoom.OnRoomEnter();

        PlayerMovement player = PlayerMovement.instance;
        player.IsFrozen = false;
        player.gameObject.SetActive(true);

        //set cam state to player follow
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.SetCameraState(CameraFollow.CameraState.Follow);
    }

    

}
