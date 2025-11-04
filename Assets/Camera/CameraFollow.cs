using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);
    private float smoothTime = 0.15f;
    private Vector3 velecoty = Vector3.zero;

    [SerializeField] private Transform target;  
    
    
    void Start()
    {
        
    }

  
    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velecoty, smoothTime);
    }
}
