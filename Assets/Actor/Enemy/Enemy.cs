using UnityEngine;
using Pathfinding;



public class Enemy : MonoBehaviour{

    [Header("Stats")]
    [SerializeField] private int maxHp = 3;
    [SerializeField] private int stamina = 5;
    [SerializeField] private int collisionDamage = 2;
    private int hp;


    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6.0f;
    [SerializeField] private float knockbackDuration = 0.15f;


    [Header("Sfx")]
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip dieSfx;


    private Rigidbody2D rb;
    private EnemyAnimator anim;
    private AudioSource audioSource;

    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity = Vector2.zero;

    [Header("Pathfinding")]
    public Transform target;
    public float moveSpeed = 2.0f;
    public float nextWaypointDistancce = 0.5f;

    private Path path;
    private int currentWaypoint = 0;
    public bool ReachEndOfPath { get; private set; }
    private Seeker seeker;


    public bool HasPath => path != null;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
        anim = GetComponent<EnemyAnimator>();
        audioSource = GetComponent<AudioSource>();

        seeker = GetComponent<Seeker>();
    }

    void Start() {
       // InvokeRepeating("UpdatePath", 0.0f, 0.5f);  
    }

 
    void FixedUpdate(){

        Vector2 moveDir = Vector2.zero;

        if (isKnockedBack){
            moveDir = knockbackVelocity;
        }
        else if (path != null) {
            if (currentWaypoint >= path.vectorPath.Count){
                ReachEndOfPath = true;
                moveDir = Vector2.zero;
                //make enemy idle
                anim.Play(ActorAnimator.ActorAnimation.Idle, ActorAnimator.FacingDirection.South, false, false);
            }
            else { 
                ReachEndOfPath=false;
                moveDir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                //make enemy walking
                anim.Play(ActorAnimator.ActorAnimation.Walk, ActorAnimator.FacingDirection.East, false, false);
            }
        }
        else{
            moveDir = Vector2.zero;
            //make enemy idle
            anim.Play(ActorAnimator.ActorAnimation.Idle, ActorAnimator.FacingDirection.South, false, false);
        }

        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
        

        if (!ReachEndOfPath && path != null){
            //check if at next waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistancce)
            {
                currentWaypoint++;
            }
            else {
                ReachEndOfPath = true;
            }
        }

    }

    public void TakeDamage(int damageAmount, Vector2 sourcePosition){
        if (isKnockedBack) return;
        audioSource.PlayOneShot(hitSfx);
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        

        hp -= damageAmount;
        if (hp <= 0) 
            Die();
        else
            ApplyKnockback(direction);
    }



    private void ApplyKnockback(Vector2 direction) {
        isKnockedBack = true;
        knockbackVelocity = direction * knockbackForce;
        anim.Play(ActorAnimator.ActorAnimation.Hit,ActorAnimator.FacingDirection.South,true,false);
        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback(){
        isKnockedBack = false;
        anim.Unlock();
        anim.Play(ActorAnimator.ActorAnimation.Idle, ActorAnimator.FacingDirection.South, false, false);

    }

    private void Die(){
        //Destroy(this.gameObject);
        Debug.Log("Enemy died");
        audioSource.PlayOneShot(dieSfx);
        anim.Play(ActorAnimator.ActorAnimation.Dying,ActorAnimator.FacingDirection.South,true,true);   
        GetComponent<CapsuleCollider2D>().enabled = false;
    }



    public void SetPath(Vector2 pos) {
        if (!seeker.IsDone()) return;
        seeker.StartPath(rb.position, pos, OnPathComplete);
    }

    private void OnPathComplete(Path p) {
        if (!p.error){
            path = p;
            currentWaypoint = 0;
        }
    }

    private void UpdatePath() {
        if (!seeker.IsDone()) return;
        if (target != null)
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        else {
            //path.path.Clear();
            
            path = null;
            //seeker.StartPath(rb.position, rb.position, OnPathComplete);

        }
            
       
        


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.TakeDamage(collisionDamage, this.gameObject.transform.position);
        }
    }



}
