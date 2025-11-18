using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);
    private float smoothTime = 0.15f;
    private Vector3 velecoty = Vector3.zero;

    [SerializeField] private Transform target;
    [SerializeField] private float maxX = 15.0f;
    [SerializeField] private float minX = -15.0f;
    [SerializeField] private float maxY = 15.0f;
    [SerializeField] private float minY = -15.0f;

    Camera cam;
    float halfHeight;
    float halfWidth;



    void Start()
    {

        Camera cam = GetComponent<Camera>();

        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * cam.aspect;

        Debug.Log(halfWidth);
        Debug.Log(halfHeight);


    }

  
    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;

      
        //testing to clamp camera into max and min xy
        if (targetPos.x >  maxX - halfWidth) targetPos.x = maxX - halfWidth;
        if (targetPos.x < minX + halfWidth) targetPos.x = minX + halfWidth;
        if (targetPos.y > maxY - halfHeight) targetPos.y = maxY-halfHeight;
        if (targetPos.y < minY + halfHeight) targetPos.y = minY + halfHeight;
        //TO-DO: Account for the camera size so the numbers are the edge of frame and not center (camera location)


        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velecoty, smoothTime);
    }
}
