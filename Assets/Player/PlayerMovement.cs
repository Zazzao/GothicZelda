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

        // Bind the Move action directly (OLD LOGIC - calling functions instead of updating varible)
        // controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        //controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

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



    // This is called automatically by Player Input when using "Send Messages"
    // NOTE: This is not being called 
    public void OnMove(InputAction.CallbackContext context){
        Debug.Log("OnMove");
        if (isFrozen) {
            moveInput = Vector2.zero;
            return;
        } 
            
            

        moveInput = context.ReadValue<Vector2>();
        moveInput = ClampToCardinal(moveInput);
        playerfacing = CalcPlayerFacing(moveInput);
        switch (playerfacing)
        {
            case PlayerFacing.North:
                anim.Play(PlayerAnimator.Animations.WALK_N, false, false);
                break;
            case PlayerFacing.East:
                anim.Play(PlayerAnimator.Animations.WALK_E, false, false);
                break;
            case PlayerFacing.South:
                anim.Play(PlayerAnimator.Animations.WALK_S, false, false);
                break;
            case PlayerFacing.West:
                anim.Play(PlayerAnimator.Animations.WALK_W, false, false);
                break;
        }

    }

    private void OnMoveCancelled(InputAction.CallbackContext context) {
        moveInput = Vector2.zero;
        switch (playerfacing)
        {
            case PlayerFacing.North:
                anim.Play(PlayerAnimator.Animations.IDLE_N, false, false);
                break;
            case PlayerFacing.East:
                anim.Play(PlayerAnimator.Animations.IDLE_E, false, false);
                break;
            case PlayerFacing.South:
                anim.Play(PlayerAnimator.Animations.IDLE_S, false, false);
                break;
            case PlayerFacing.West:
                anim.Play(PlayerAnimator.Animations.IDLE_W, false, false);
                break;
        }
        
    }


    private void OnAttack(InputAction.CallbackContext context) {
        isAttacking = true;
        //do attack animation
        switch (playerfacing)
        {
            case PlayerFacing.North:
                anim.Play(PlayerAnimator.Animations.ATTACK_N, true, false);
                break;
            case PlayerFacing.East:
                anim.Play(PlayerAnimator.Animations.ATTACK_E, true, false);
                break;
            case PlayerFacing.South:
                anim.Play(PlayerAnimator.Animations.ATTACK_S, true, false);
                break;
            case PlayerFacing.West:
                anim.Play(PlayerAnimator.Animations.ATTACK_W, true, false);
                break;

        }

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
        
        //DEV NOTE: this is just for early testing - this is not the way to decide to play walk anim
        if (moveInput != Vector2.zero){
            isWalking = true;
        }else { 
            isWalking = false;
        }


        Vector2 normalizedMove = moveInput.normalized;
        if (isAttacking) normalizedMove = Vector2.zero; //cant move while attacking
        rb.MovePosition(rb.position + normalizedMove * moveSpeed * Time.fixedDeltaTime);


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
        playerfacing = CalcPlayerFacing(moveInput);

        PlayerAnimator target = this.GetComponent<PlayerAnimator>(); // this mean we have to have a specific on exit for each type of 
        target.SetLocked(false);

        if (isWalking)
        {
           
            switch (playerfacing)
            {
               
                case PlayerFacing.North:
                    anim.Play(PlayerAnimator.Animations.WALK_N, false, false);
                    break;
                case PlayerFacing.East:
                    anim.Play(PlayerAnimator.Animations.WALK_E, false, false);
                    break;
                case PlayerFacing.South:
                    anim.Play(PlayerAnimator.Animations.WALK_S, false, false);
                    break;
                case PlayerFacing.West:
                    anim.Play(PlayerAnimator.Animations.WALK_W, false, false);
                    break;
            }

        }
        else {
            switch (playerfacing)
            {
                case PlayerFacing.North:
                    anim.Play(PlayerAnimator.Animations.IDLE_N, false, false);
                    break;
                case PlayerFacing.East:
                    anim.Play(PlayerAnimator.Animations.IDLE_E, false, false);
                    break;
                case PlayerFacing.South:
                    anim.Play(PlayerAnimator.Animations.IDLE_S, false, false);
                    break;
                case PlayerFacing.West:
                    anim.Play(PlayerAnimator.Animations.IDLE_W, false, false);
                    break;
            }


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

}
