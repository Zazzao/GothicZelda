using System;
using DensetsuEngine.Utils;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Sensor : MonoBehaviour{

    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float timerInterval = 1.0f;

    private CircleCollider2D detectionRange;

    public event Action OnTargetChanged = delegate { };

    public Vector2 TargetPosition => target ? target.transform.position : Vector2.zero;
    public bool IsTargetInRange => TargetPosition != Vector2.zero;

    private GameObject target;
    private Vector2 lastKnownPosition;
    CountdownTimer timer;

    private void Awake()
    {
        detectionRange = GetComponent<CircleCollider2D>();
        detectionRange.isTrigger = true;
        detectionRange.radius = detectionRadius;


    }

    private void Start()
    {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () =>
        {
            UpdateTargetPosition(target); // need to add thier externtion for .OrNull()
            timer.Start();
        };
        timer.Start();
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        UpdateTargetPosition(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        UpdateTargetPosition();
    }

    private void UpdateTargetPosition(GameObject target = null) { 
        this.target = target;
        if (IsTargetInRange && (lastKnownPosition != TargetPosition || lastKnownPosition != Vector2.zero)) { 
            lastKnownPosition = TargetPosition;
            OnTargetChanged.Invoke();
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
