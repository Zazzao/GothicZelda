using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    public static PlayerMovement instance;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private PlayerFacing playerfacing = PlayerFacing.South;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isFrozen = false;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6.0f;
    [SerializeField] private float knockbackDuration = 0.15f;

    [SerializeField] private float invulnDuration = 0.5f;
    private bool isInvulnerable = false;



    private bool isKnockedBack;
    private Vector2 knockbackVelocity;

    private Rigidbody2D rb;                 //player motor
    private Vector2 moveInput;              //player controller
    private PlayerInputActions controls;    //player controller

    private PlayerAnimator anim;            //player motor should tell player animator

    public enum PlayerFacing{North,East,South,West} //move to a static helper class (all actors can have a facing/direction)


    public bool IsFrozen { set { isFrozen = value; } get { return isFrozen; } }


    private void Awake(){
        
        instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimator>();
        anim.Initialize();

        controls = new PlayerInputActions();

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMoveCancelled;
        controls.Player.Attack.performed += OnAttack;

    }

    #region On Enable/Disable
    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }
    #endregion




    public void OnMove(InputAction.CallbackContext context){
        //Debug.Log("OnMove");
        if (isFrozen) {
            moveInput = Vector2.zero;
            return;
        } 
        
        moveInput = context.ReadValue<Vector2>();
        moveInput = ClampToCardinal(moveInput);
        if (moveInput != Vector2.zero) playerfacing = CalcPlayerFacing(moveInput);
        anim.PlayWalkAnimation(playerfacing,false,false);

    }

    private void OnMoveCancelled(InputAction.CallbackContext context) {
        moveInput = Vector2.zero;
        anim.PlayIdleAnimation(playerfacing,false,false);
   
    }


    private void OnAttack(InputAction.CallbackContext context) {
        isAttacking = true;
        //do attack animation
        anim.PlayAttackAnimation(playerfacing,true,false);

    }

    private void Update() {

        //Debug Animation Testing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //kill player
            anim.Play(PlayerAnimator.Animations.DYING, true, true);
            enabled = false;
        }


    }


    private void FixedUpdate(){
        
        isWalking = moveInput != Vector2.zero;

        Vector2 moveDir;

        if (isKnockedBack)
        {
            moveDir = knockbackVelocity;
            //Debug.DrawRay(transform.position, knockbackVelocity, Color.magenta, 0.2f);
        }
        else if (isAttacking)
        {
            moveDir = Vector2.zero;
        }
        else moveDir = moveInput * moveSpeed;
        {
            rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
        }

    }


    private PlayerFacing CalcPlayerFacing(Vector2 vector) {
        

        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            // Vector is more horizontal
            if (vector.x > 0){
                return PlayerFacing.East;
            }else{
                return PlayerFacing.West;
            }
        }else {
            // Vector is more vertical
            if (vector.y > 0){
                return PlayerFacing.North;
            }else{
                return PlayerFacing.South;
            }
        }

    }

    public void OnAttackEnd() {
        isAttacking = false;

        PlayerAnimator target = this.GetComponent<PlayerAnimator>(); // this mean we have to have a specific on exit for each type of 
        target.SetLocked(false);

        if (isWalking){
            anim.PlayWalkAnimation(playerfacing, false, false);
        }
        else {
            anim.PlayIdleAnimation(playerfacing,false,false);
        }
    }

    private Vector2 ClampToCardinal(Vector2 input, float deadzone = 0.2f) {
        if (input.magnitude < deadzone) return Vector2.zero;

        //Get angel in degrees (0 = right)
        float angle = Mathf.Atan2 (input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        //snap angle to nearest 45 degree
        float snappedAngle = Mathf.Round(angle / 45.0f) * 45.0f;

        //convert back to vector
        float rad = snappedAngle * Mathf.Deg2Rad;
        Vector2 snapped = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        return snapped.normalized;

    }

    public void TakeDamage(int damageAmount,Vector2 sourcePosition) {

        if (isKnockedBack) return;
        if (isInvulnerable) return;

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        ApplyKnockback(direction);
        HealthDisplay_Hearts.heartHealthSystemStatic.Damage(damageAmount);

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
        //anim.playHitAnimation(playerfacing, true, true);

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback() { 
        isKnockedBack = false;

        if (isWalking)
            anim.PlayWalkAnimation(playerfacing,false,false);
        else
            anim.PlayIdleAnimation(playerfacing,false,false);

    }


    private void EndInvulnerability()
    {
        isInvulnerable = false;
    }




}
