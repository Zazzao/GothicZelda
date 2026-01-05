using UnityEngine;
using UnityEngine.UIElements;

public class Room : MonoBehaviour
{

    [Header("Camera Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public GameObject go_minX;
    public GameObject go_minY;
    public GameObject go_maxX;
    public GameObject go_maxY;





    [SerializeField] private BoxCollider2D camBounds;

    [Header("Room Contents")]
    [SerializeField] private GameObject[] roomObjects;


    private void Awake()
    {
        camBounds = GetComponent<BoxCollider2D>();
        camBounds.isTrigger = true;

        minX = go_minX.transform.position.x;
        maxX = go_maxX.transform.position.x;
        minY = go_minY.transform.position.y;
        maxY = go_maxY.transform.position.y;

        
    }


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


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        //Gizmos.DrawCube(transform.position, camBounds.size);

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireCube(transform.position, camBounds.size);
    }
#endif





}
