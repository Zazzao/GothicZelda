using UnityEngine;

public abstract class ActorAnimator : MonoBehaviour
{
    protected Animator anim;
    protected bool animationLocked = false;
    protected ActorAnimation currentAction;
    protected FacingDirection currentFacing;


    public enum FacingDirection { 
        North,
        East,
        South,
        West
    }

    public enum ActorAnimation {
        Idle,
        Walk,
        Attack,
        Roll,
        Hit,
        Knockback,
        Dying,
        Dead
    }



    protected virtual void Awake() { 
        anim = GetComponent<Animator>();
    }


    public void Play(ActorAnimation action, FacingDirection facing, bool lockAnimation, bool bypassLock) {
        if (animationLocked && !bypassLock) return;

        if (lockAnimation)
            animationLocked = true;

        currentAction = action;
        currentFacing = facing;

        int hash = GetAnimationHash(action, facing);
        anim.CrossFade(hash, 0.0f);

    }

    public void Unlock(){
        animationLocked = false;
    }


    protected int GetAnimationHash(ActorAnimation action, FacingDirection facing){
        if (IsDirectional(action))
            return Animator.StringToHash($"{action}_{FacingToSuffix(facing)}");

        return Animator.StringToHash(action.ToString());

    }


    protected virtual bool IsDirectional(ActorAnimation action){
        switch (action){
            case ActorAnimation.Idle:
            case ActorAnimation.Walk:
            case ActorAnimation.Attack:
            case ActorAnimation.Roll:
                return true;

            case ActorAnimation.Hit:
            case ActorAnimation.Dying:
            case ActorAnimation.Dead:
                return false;

            default:
                return false;
        }
    }



    protected string FacingToSuffix(FacingDirection facing){
        return facing switch{
            FacingDirection.North => "N",
            FacingDirection.East => "E",
            FacingDirection.South => "S",
            FacingDirection.West => "W",
            _ => "S"
        };
    }



}
