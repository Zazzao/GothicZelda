using UnityEngine;

public class Room : MonoBehaviour
{

    [Header("Camera Bounds")]
    public float minX, maxX, minY, maxY;

    [Header("Room Contents")]
    [SerializeField] private GameObject[] roomObjects;

    public void OnRoomEnter(){
        SetRoomActive(true);
    }

    public void OnRoomExit(){
        SetRoomActive(false);
    }

    private void SetRoomActive(bool active){
        foreach (var obj in roomObjects){
            obj.SetActive(active);
        }
    }


}
