using UnityEngine;

public class PlayerMotor : MonoBehaviour {


    public static PlayerMotor Instance;

    private Rigidbody2D rb;
    private PlayerAnimator anim;
    private AudioSource audioSource;


    [Header("Player Vitals")]
    [SerializeField] private int hp = 0;
    [SerializeField] private int maxHp = 20;
    public Vital Stamina;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private ActorAnimator.FacingDirection facing;
    private Vector2 moveInput;
    private bool isFrozen = false; //not used yet

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6.0f;
    [SerializeField] private float knockbackDuration = 0.15f;
    //private bool isKnockedBack;
    private Vector2 knockbackVelocity;


    [Header("Attack Movement")]
    [SerializeField] private float attackLungeForce = 3.0f;
    private bool isLunging = false;
    private Vector2 attackDir;

    [Header("I-Frames")]
    [SerializeField] private float invulnDuration = 0.5f;
    private bool isInvulnerable = false;


    [Header("Dodge Roll")]
    [SerializeField] private float rollSpeed = 8.0f;
    [SerializeField] private float rollDuration = 0.25f;    // THIS IS NOT BEING USED (ROLL END AT ANIMATION END)
    [SerializeField] private float rollStaminaCost = 25.0f;
    //private bool isRolling;
    private Vector2 rollDirection;


    [Header("Sfx")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip rollSfx;


    public enum MovementMode{
        TopDown,
        SideScroller,
        PointClick
    }

    public enum MovementState{
        Idle,
        Walking,
        Rolling,
        Knockback,
        Jumping,
        Falling,
        Burrowing
    }

    public enum ActionState{
        None,
        Attacking,
        HeavyAttack,
        Casting,
        Interacting,
        UsingItem
    }

    private MovementMode movementMode = MovementMode.TopDown;
    private MovementState playerMovementState = MovementState.Idle;
    private ActionState playerActionState = ActionState.None;


    //Status Effects
    public bool IsFrozen { get; set; }
    public bool IsInvulnerable { get; private set; }
    public bool IsDead { get; private set; }



    public ActorAnimator.FacingDirection CurrentFacing { get { return facing; } }



    private void Awake() {

        InitializeSingleton();
        InitializeAttachedComponents();

        //NOTE: THIS SHOULD BE MOVED
        hp = maxHp;
        Stamina = new Vital("Stamina", 100, 15f, 0.75f);
        FindAnyObjectByType<StaminaBarUI>().Bind(Stamina);


    }


    private void Start(){
        //setup hearth health system
        HeartHealthSystem heartHealthSystem = new HeartHealthSystem((int)maxHp / 4); //get number of hearts based on hp (4 fragments per heart)
        GameObject.FindAnyObjectByType<HealthDisplay_Hearts>().SetHeartHealthSystem(heartHealthSystem);

    }

    private void Update(){
        Stamina.Tick(Time.deltaTime);
        DEBUG_HOTKEYS();
    }


    private void FixedUpdate(){

        Vector2 moveDir;
        switch (playerMovementState){
            case MovementState.Idle:
                moveDir = Vector2.zero;
                break;
            case MovementState.Walking:
                moveDir = moveInput * moveSpeed;
                break;
            case MovementState.Rolling:
                moveDir = rollDirection * rollSpeed;
                break;
            case MovementState.Knockback:
                moveDir = knockbackVelocity;
                break;
        }


        if (playerMovementState == MovementState.Knockback){
            moveDir = knockbackVelocity;
            //Debug.DrawRay(transform.position, knockbackVelocity, Color.magenta, 0.2f);
        }
        else if (playerActionState == ActionState.Attacking|| IsFrozen){
            if (isLunging){
                moveDir = attackDir * attackLungeForce;
            }
            else{
                moveDir = Vector2.zero;
            }
        }
        else if (IsFrozen){
            moveDir = Vector2.zero;
        }
        else if (playerMovementState == MovementState.Rolling){
            moveDir = rollDirection * rollSpeed;
        }
        else{
            moveDir = moveInput * moveSpeed;
        }
        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
    }




    public MovementState GetMovementState() {
        return playerMovementState;
    }

    private void EnterMovementState(MovementState newState) {
       // if (playerMovementState == newState) return; 
       //NOTE: same movement state with diff face needs anim update cant just return is new state is the same

        playerMovementState = newState;
        switch (playerMovementState){
            case MovementState.Idle:
                anim.PlayIdle(facing);
                break;
            case MovementState.Walking:
                anim.PlayWalk(facing);
                break;
            case MovementState.Rolling:
                anim.PlayRoll(facing);
                break;

        }

        //Debug.Log($"Movement State -> {playerMovementState}");

    }

    private void UpdateMovementState()
    {
        if (moveInput == Vector2.zero)
            EnterMovementState(MovementState.Idle);
        else
            EnterMovementState(MovementState.Walking);
    }







    private void EnterActionState(ActionState newActionState) { 
        if (playerActionState == newActionState) return;

        playerActionState = newActionState;
    }



    public void OnMove(Vector2 v) {

        moveInput = ClampToCardinal(v);

        if (IsFrozen) moveInput = Vector2.zero;

        if (moveInput == Vector2.zero) { 
            EnterMovementState(MovementState.Idle);
            return;
        }

        facing = CalcPlayerFacing(moveInput);
        EnterMovementState(MovementState.Walking);

        
    }

    public void OnMoveCancelled() {
        moveInput = Vector2.zero;
        EnterMovementState(MovementState.Idle);
    }

    public void OnAttack() {
        if (IsFrozen || playerMovementState == MovementState.Rolling || playerActionState == ActionState.Attacking) return;
        OnAttackStart();
    }

    public void OnRoll() {
        Debug.Log("roll input performed");

        if (IsFrozen || playerMovementState == MovementState.Rolling || playerActionState == ActionState.Attacking) return;
        if (Stamina.Current < rollStaminaCost) return;

        Stamina.Spend(rollStaminaCost);
        OnRollStart();
    }


    public void OnInteract() {
        Interactable.TryInteract();
    }

    private void OnDeath(){
        //kill player
        anim.PlayDying(ActorAnimator.FacingDirection.South);
        enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }





    private void OnAttackStart(){
        EnterActionState(ActionState.Attacking);
        audioSource.PlayOneShot(attackSfx);
        anim.PlayAttack(facing);
    }


    public void OnAttackEnd(){
        EnterActionState(ActionState.None);
        anim.Unlock();
        UpdateMovementState();

    }


    //this is called by the animator and add the "lunge" movement to the player
    public void OnAttackLunge(){
        isLunging = true;
        attackDir = FacingToVector(CurrentFacing);

    }

    public void OnAttackLungeEnd(){
        isLunging = false;
    }



    private void OnRollStart()
    {
        EnterMovementState(MovementState.Rolling);
        IsInvulnerable = true;          // note: might want to handle i-frames in animation
        rollDirection = moveInput;      // note: might not need this
        audioSource.PlayOneShot(rollSfx);
        

    }


    public void OnRollEnd(){
        IsInvulnerable = false;
        anim.Unlock();
        UpdateMovementState();

    }




    private void EndInvulnerability(){
        IsInvulnerable = false;
    }

    private void InitializeSingleton() {
        if (Instance != null){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeAttachedComponents() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimator>();
        audioSource = GetComponent<AudioSource>();
    }


    private void DEBUG_HOTKEYS() {
        //temp logic for gameplay testing

        //Debug Animation Testing
        if (Input.GetKeyDown(KeyCode.Q)) OnDeath();

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); //debug

        //TO-DO: Move this to a Debug Manager
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int layer = LayerMask.NameToLayer("System Gizmo");
            int mask = 1 << layer;

            bool isVisible = (Camera.main.cullingMask & mask) != 0;

            if (isVisible)
                Camera.main.cullingMask &= ~mask; // turn OFF
            else
                Camera.main.cullingMask |= mask;  // turn ON

        }
    }




    private Vector2 FacingToVector(ActorAnimator.FacingDirection facing){
        return facing switch{
            ActorAnimator.FacingDirection.North => Vector2.up,
            ActorAnimator.FacingDirection.South => Vector2.down,
            ActorAnimator.FacingDirection.East => Vector2.right,
            ActorAnimator.FacingDirection.West => Vector2.left,
            _ => Vector2.zero
        };
    }



    public void TakeDamage(int damageAmount, Vector2 sourcePosition)
    {

        if (playerMovementState == MovementState.Knockback) return;
        if (isInvulnerable) return;

        audioSource.PlayOneShot(hitSfx);


        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        ApplyKnockback(direction);
        HealthDisplay_Hearts.heartHealthSystemStatic.Damage(damageAmount);
        hp -= damageAmount;
        if (hp <= 0)
        {
            OnDeath();
        }

        isInvulnerable = true;
        Invoke(nameof(EndInvulnerability), invulnDuration); //TO-DO: make I frames in the animation and not a "timed" thing

    }

    public void Heal(int healAmount)
    {
        HealthDisplay_Hearts.heartHealthSystemStatic.Heal(healAmount);
    }

    private void ApplyKnockback(Vector2 direction)
    {
        
        EnterActionState(ActionState.None);
        EnterMovementState(MovementState.Knockback);
        moveInput = Vector2.zero;

        knockbackVelocity = direction * knockbackForce;

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback(){
        EnterActionState(ActionState.None);
        UpdateMovementState();

    }



    public ActorAnimator.FacingDirection GetFacing() {
        return facing;
    }



    #region Math Functions
    private ActorAnimator.FacingDirection CalcPlayerFacing(Vector2 vector)
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

    private Vector2 ClampToCardinal(Vector2 input, float deadzone = 0.2f){
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
