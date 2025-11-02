using System.Collections;
using UnityEngine;

public class OnExit : StateMachineBehaviour
{

    //OBSOLET CODE: using animation events to trigger this now


    [SerializeField] private PlayerAnimator.Animations animation;
    [SerializeField] private bool lockAnimation;
    [HideInInspector] public bool cancel = false;


    //OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cancel = false;
        PlayerMovement.instance.StartCoroutine(Wait()); //requires ANY static instance -- should prob move to gamemanager type of obj at some point

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(stateInfo.length);
            if (cancel) yield break;

            PlayerAnimator target = animator.GetComponent<PlayerAnimator>(); // this mean we have to have a specific on exit for each type of 
            target.SetLocked(false);

            target.Play(animation, lockAnimation, false);

        }

    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
