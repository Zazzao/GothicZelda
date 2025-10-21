using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private PlayerFacing playerfacing = PlayerFacing.South;


    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerInputActions controls;

    public enum PlayerFacing{North,East,South,West}


    private void Awake(){
        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerInputActions();

        // Bind the Move action directly
       // controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        //controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMoveCancelled;

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
        Debug.Log("OnMoveCalled");
        moveInput = context.ReadValue<Vector2>();
        playerfacing = CalcPlayerFacing(moveInput);

    }

    private void OnMoveCancelled(InputAction.CallbackContext context) {
        moveInput = Vector2.zero;
    }



    private void FixedUpdate(){

        Vector2 normalizedMove = moveInput.normalized;
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

}
