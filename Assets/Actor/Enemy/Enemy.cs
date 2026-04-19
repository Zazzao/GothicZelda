using UnityEngine;
using Pathfinding;
using DensetsuEngine.GOAP;



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
    [SerializeField] public bool ReachEndOfPath; //{ get; set; }
    private Seeker seeker;

    private ActorAnimator.FacingDirection facing = ActorAnimator.FacingDirection.South;


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

        Vector2 moveDir;

        if (isKnockedBack){
            moveDir = knockbackVelocity;
        }
        else if (path != null && currentWaypoint < path.vectorPath.Count) {
            moveDir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            facing = CalcEnemyFacing(moveDir);
            anim.Play(ActorAnimator.ActorAnimation.Walk, facing, false, false);  
        }
        else{
            moveDir = Vector2.zero;
            anim.Play(ActorAnimator.ActorAnimation.Idle, facing, false, false);
        }

        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
        
        if (path != null){
            //check if at next waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistancce)currentWaypoint++;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                ReachEndOfPath = true;
                path = null;
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
        
        GetComponent<GoapAgent>().enabled = false;  //this only stops the plan does not stop movement
    }



    public void SetPath(Vector2 pos) {
        if (!seeker.IsDone()) return;
        seeker.StartPath(rb.position, pos, OnPathComplete);
        ReachEndOfPath = false;
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


    //#region Math Functions
    private ActorAnimator.FacingDirection CalcEnemyFacing(Vector2 vector)
    {


        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            // Vector is more horizontal
            if (vector.x > 0)
            {
                return ActorAnimator.FacingDirection.East;
            }
            else
            {
                return ActorAnimator.FacingDirection.West;
            }
        }
        else
        {
            // Vector is more vertical
            if (vector.y > 0)
            {
                return ActorAnimator.FacingDirection.North;
            }
            else
            {
                return ActorAnimator.FacingDirection.South;
            }
        }


    }






}
