using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);
    private float smoothTime = 0.15f;
    private Vector3 velecoty = Vector3.zero;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector2 cameraPanTarget = Vector2.zero;
    [SerializeField] private float maxX = 15.0f;
    [SerializeField] private float minX = -15.0f;
    [SerializeField] private float maxY = 15.0f;
    [SerializeField] private float minY = -15.0f;

    Camera cam;
    float halfHeight;
    float halfWidth;

    public Room startingRoom;

    public enum CameraState{
        Follow,
        Transition
    }
    private CameraState state = CameraState.Follow;




    private void Awake() {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * cam.aspect;

       // cam.transparencySortMode = TransparencySortMode.Orthographic;

    }

    private void Start()
    {
        playerTransform = PlayerMovement.instance.transform;
        SetCameraBounds(startingRoom.minX, startingRoom.maxX, startingRoom.minY, startingRoom.maxY);

        Vector3 targetPos = playerTransform.position + offset;
        targetPos = CalcClampedCameraPosition(targetPos);


        cam.transform.position = targetPos;


    }


    void LateUpdate(){

        if (playerTransform == null) return;

        switch (state) { 
            case CameraState.Follow: OnCamFollow(); break;
            case CameraState.Transition: OnCamTransition(); break;
        
        }
        
    }

    private void OnCamFollow() {
        // follow the player while clamping to room bounds
        Vector3 targetPos = playerTransform.position + offset;
        targetPos = CalcClampedCameraPosition(targetPos);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velecoty, smoothTime);
    }

    private void OnCamTransition() {
        //cam is doing a room transition pan


        transform.position = Vector3.SmoothDamp(transform.position, (Vector3)cameraPanTarget + offset,ref velecoty,smoothTime);

        // Stop transitioning when close enough
        if (Vector3.Distance(transform.position, (Vector3)cameraPanTarget + offset) < 0.05f){
            transform.position = (Vector3)cameraPanTarget + offset;
            RoomTransitionManager.instance.EndTransition();
        }


    }


    private Vector3 CalcClampedCameraPosition(Vector3 playerPos) { 
    
        
        
        float clampedX = Mathf.Clamp(playerPos.x, minX+halfWidth, maxX-halfWidth);
        float clampedY = Mathf.Clamp(playerPos.y, minY+halfHeight, maxY-halfHeight);

        if (minY + halfHeight > maxY - halfHeight) {
            clampedY = (minY + maxY) / 2.0f;
        }
        if (minX + halfWidth > maxX - halfWidth)
        {
            clampedX = (minX + maxX) / 2.0f;
        }


        return new Vector3(clampedX, clampedY,offset.z);
    
    }


    public void SetCameraBounds(float newMinX, float newMaxX, float newMinY, float newMaxY) { 
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
    }


    public void SetCameraState(CameraState newState) { 
        state = newState;
    }

    public void SetPanTartget(Vector2 newPanPosition) {

        cameraPanTarget = newPanPosition;

        float clampedX = Mathf.Clamp(cameraPanTarget.x, minX + halfWidth, maxX - halfWidth);
        float clampedY = Mathf.Clamp(cameraPanTarget.y, minY + halfHeight, maxY - halfHeight);

        if (minY + halfHeight > maxY - halfHeight)
        {
            clampedY = (minY + maxY) / 2.0f;
        }
        if (minX + halfWidth > maxX - halfWidth)
        {
            clampedX = (minX + maxX) / 2.0f;
        }

        cameraPanTarget = new Vector2(clampedX, clampedY);
    }





}
