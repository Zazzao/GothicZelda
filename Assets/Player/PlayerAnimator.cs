using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public readonly static int[] animations = {
        Animator.StringToHash("Idle_N"),
        Animator.StringToHash("Idle_E"),
        Animator.StringToHash("Idle_S"),
        Animator.StringToHash("Idle_W"),
        Animator.StringToHash("Walk_N"),
        Animator.StringToHash("Walk_E"),
        Animator.StringToHash("Walk_S"),
        Animator.StringToHash("Walk_W"),
        Animator.StringToHash("Dying"),
        Animator.StringToHash("Dead")

    };

    private Animator anim;
    private Animations currentAnimation;
    private bool animationLocked;

    // DEV NOTE: this was orginally outside of class - im putting it inside so its spec to each animator class
    public enum Animations { 
        IDLE_N,
        IDLE_E,
        IDLE_S,
        IDLE_W,
        WALK_N,
        WALK_E,
        WALK_S,
        WALK_W,
        DYING,
        DEAD,
        NONE
    }


    public void Initialize() { 
        animationLocked = false;
        currentAnimation = Animations.WALK_E;
        anim = GetComponent<Animator>();
    }

    public Animations GetCurrentAnimation() { 
        return currentAnimation;
    }

    public void SetLocked(bool isLocked) { 
        animationLocked = isLocked;
    }

    public void Play(Animations animation, bool isLocked, bool bypassLock) {

        if (animation == Animations.NONE) {
            DefaultAnimation();
            return;
        }

        if (animationLocked && !bypassLock) return;
        animationLocked = isLocked;

        if (currentAnimation == animation) return;

        // update the animation
        currentAnimation = animation;
        anim.CrossFade(animations[(int)currentAnimation], 0.0f, 0); 

    }


    private void DefaultAnimation() { 
        //DEV NOTE: this should actually be on the playermovement script as it req knowing direction and inputs
    }



}
