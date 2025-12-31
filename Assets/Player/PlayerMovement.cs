using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    public static PlayerMovement instance;

    [Header("Player Vitals")]
    [SerializeField] private int hp = 0;
    [SerializeField] private int maxHp = 20;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private ActorAnimator.FacingDirection facing;
    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isFrozen = false; //not used yet

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6.0f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private bool isKnockedBack;
    private Vector2 knockbackVelocity;

    [Header("I-Frames")]
    [SerializeField] private float invulnDuration = 0.5f;
    private bool isInvulnerable = false;


    private Rigidbody2D rb;                 
    private Vector2 moveInput;              
    private PlayerInputActions controls;   
    private PlayerAnimator anim;           

    //public enum PlayerFacing{North,East,South,West} //move to a static helper class (all actors can have a facing/direction)


    public bool IsFrozen { set { isFrozen = value; } get { return isFrozen; } }

    public ActorAnimator.FacingDirection CurrentFacing { get { return facing; } }
   

    #region On Enable/Disable
    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }
    #endregion


    #region Input Functions
    public void OnMoveInputPerformed(InputAction.CallbackContext context){
 
        if (isFrozen) {
            moveInput = Vector2.zero;
            return;
        } 
        
        moveInput = context.ReadValue<Vector2>();
        moveInput = ClampToCardinal(moveInput);
        if (moveInput != Vector2.zero) facing = CalcPlayerFacing(moveInput);
        anim.Play(ActorAnimator.ActorAnimation.Walk, facing, false, false);

    }

    private void OnMoveInputCancelled(InputAction.CallbackContext context) {
        moveInput = Vector2.zero;
        anim.Play(ActorAnimator.ActorAnimation.Idle, facing, false, false);
    }

    private void OnAttackInputPerformed(InputAction.CallbackContext context) {
        if (isFrozen) return;
        
        isAttacking = true;
        anim.Play(ActorAnimator.ActorAnimation.Attack, facing, true, false);
    }

    private void OnInteractInputPerformed(InputAction.CallbackContext context) {
        Debug.Log("Interact button pressed");
        Interactable.TryInteract();
    }

    #endregion


    #region Game Loop Functions

    private void Awake(){

        instance = this;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimator>();

        controls = new PlayerInputActions();

        controls.Player.Move.performed += OnMoveInputPerformed;
        controls.Player.Move.canceled += OnMoveInputCancelled;
        controls.Player.Attack.performed += OnAttackInputPerformed;
        controls.Player.Interact.performed += OnInteractInputPerformed;

        hp = maxHp;
        

    }

    private void Start()
    {
        //setup hearth health system
        HeartHealthSystem heartHealthSystem = new HeartHealthSystem((int)maxHp/4); //get number of hearts based on hp (4 fragments per heart)
        GameObject.FindAnyObjectByType<HealthDisplay_Hearts>().SetHeartHealthSystem(heartHealthSystem);
    }


    private void Update() {

        //Debug Animation Testing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnDeath();  
        }


    }

    private void FixedUpdate(){
        
        isWalking = moveInput != Vector2.zero;

        Vector2 moveDir;

        if (isKnockedBack){
            moveDir = knockbackVelocity;
            //Debug.DrawRay(transform.position, knockbackVelocity, Color.magenta, 0.2f);
        }
        else if (isAttacking){
            moveDir = Vector2.zero;
        }
        else {
            moveDir = moveInput * moveSpeed;
        }
        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
    }

    #endregion


    public void OnAttackEnd() {
        isAttacking = false;
        anim.Unlock();

        if (isWalking){
            anim.Play(ActorAnimator.ActorAnimation.Walk, facing, false, false);
        }
        else {
            anim.Play(ActorAnimator.ActorAnimation.Idle, facing, false, false); 
        }
    }

    public void TakeDamage(int damageAmount,Vector2 sourcePosition) {

        if (isKnockedBack) return;
        if (isInvulnerable) return;

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        ApplyKnockback(direction);
        HealthDisplay_Hearts.heartHealthSystemStatic.Damage(damageAmount);
        hp -= damageAmount;
        if (hp <= 0) { 
           OnDeath();
        }

        isInvulnerable = true;
        Invoke(nameof(EndInvulnerability), invulnDuration); //TO-DO: make I frames in the animation and not a "timed" thing

    }
    
    public void Heal(int healAmount) {
        HealthDisplay_Hearts.heartHealthSystemStatic.Heal(healAmount);
    }

    private void ApplyKnockback(Vector2 direction) { 
        isKnockedBack = true;
        isAttacking = false;
        moveInput = Vector2.zero;

        knockbackVelocity = direction * knockbackForce;

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback() { 
        isKnockedBack = false;

        if (isWalking)
            anim.Play(ActorAnimator.ActorAnimation.Walk, facing, false, false);
        else
            anim.Play(ActorAnimator.ActorAnimation.Idle, facing, false, false);

    }

    private void OnDeath() {
        //kill player
        anim.Play(ActorAnimator.ActorAnimation.Dying, ActorAnimator.FacingDirection.South, true, true);
        enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    private void EndInvulnerability(){
        isInvulnerable = false;
    }



    #region Math Functions
    private ActorAnimator.FacingDirection CalcPlayerFacing(Vector2 vector){


        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y)){
            // Vector is more horizontal
            if (vector.x > 0){
                return ActorAnimator.FacingDirection.East;
            }
            else{
                return ActorAnimator.FacingDirection.West;
            }
        }
        else{
            // Vector is more vertical
            if (vector.y > 0){
                return ActorAnimator.FacingDirection.North;
            }
            else{
                return ActorAnimator.FacingDirection.South;
            }
        }


    }

    private Vector2 ClampToCardinal(Vector2 input, float deadzone = 0.2f)
    {
        if (input.magnitude < deadzone) return Vector2.zero;

        //Get angel in degrees (0 = right)
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        //snap angle to nearest 45 degree
        float snappedAngle = Mathf.Round(angle / 45.0f) * 45.0f;

        //convert back to vector
        float rad = snappedAngle * Mathf.Deg2Rad;
        Vector2 snapped = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        return snapped.normalized;

    }

    #endregion

}
