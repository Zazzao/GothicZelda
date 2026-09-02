using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour{

    private PlayerInputActions controls;
    private PlayerMotor motor;


    private void OnEnable(){controls.Enable();}

    private void OnDisable(){controls.Disable();}



    void Awake(){
        controls = new PlayerInputActions();

        controls.Player.Move.performed += OnMoveInputPerformed;
        controls.Player.Move.canceled += OnMoveInputCancelled;
        controls.Player.Attack.performed += OnAttackInputPerformed;
        controls.Player.Interact.performed += OnInteractInputPerformed;
        controls.Player.Roll.performed += OnRollInputPerformed;

        motor = GetComponent<PlayerMotor>();
    }


    public void OnMoveInputPerformed(InputAction.CallbackContext context){
        motor.OnMove(context.ReadValue<Vector2>());
    }

    private void OnMoveInputCancelled(InputAction.CallbackContext context){
        motor.OnMoveCancelled();
    }

    private void OnAttackInputPerformed(InputAction.CallbackContext context){
       motor.OnAttack();
    }

    private void OnInteractInputPerformed(InputAction.CallbackContext context){
        motor.OnInteract();
    }

    private void OnRollInputPerformed(InputAction.CallbackContext context){
        motor.OnRoll();
    }






}
