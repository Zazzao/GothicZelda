using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static PlayerMovement;

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
        Animator.StringToHash("Attack_N"),
        Animator.StringToHash("Attack_E"),
        Animator.StringToHash("Attack_S"),
        Animator.StringToHash("Attack_W"),
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
        ATTACK_N,
        ATTACK_E,
        ATTACK_S,
        ATTACK_W,
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

        if (animationLocked && !bypassLock)
        {
            Debug.Log("cant play anim");
            return;
        }
        animationLocked = isLocked;

        if (bypassLock) {
            foreach (var item in anim.GetBehaviours<OnExit>()) {
                item.cancel = true;
            }
        }


        if (currentAnimation == animation) return;

        // update the animation
        currentAnimation = animation;
        anim.CrossFade(animations[(int)currentAnimation], 0.0f, 0); 

    }


    private void DefaultAnimation() { 
        //DEV NOTE: this should actually be on the playermovement script as it req knowing direction and inputs
    }

    public void PlayIdleAnimation(PlayerMovement.PlayerFacing facing, bool isLocked, bool bypassLock) {

        Animations a = Animations.IDLE_S;
        switch (facing)
        {
            case PlayerFacing.North:
                a = Animations.IDLE_N;
                break;
            case PlayerFacing.East:
                a = Animations.IDLE_E;
                break;
            case PlayerFacing.South:
                a = Animations.IDLE_S;
                break;
            case PlayerFacing.West:
                a = Animations.IDLE_W;
                break;
        }

        Play(a, isLocked, bypassLock);
    }

    public void PlayWalkAnimation(PlayerMovement.PlayerFacing facing, bool isLocked, bool bypassLock) {

        Animations a = Animations.WALK_S;
        switch (facing)
        {

            case PlayerFacing.North:
                a = Animations.WALK_N;
                break;
            case PlayerFacing.East:
                a = Animations.WALK_E;
                break;
            case PlayerFacing.South:
                a = Animations.WALK_S;
                break;
            case PlayerFacing.West:
                a = Animations.WALK_W;
                break;
        }
        Play(a, isLocked, bypassLock);
    }

    public void PlayAttackAnimation(PlayerMovement.PlayerFacing facing, bool isLocked, bool bypassLock) {
        Animations a = Animations.ATTACK_S;
        switch (facing)
        {

            case PlayerFacing.North:
                a = Animations.ATTACK_N;
                break;
            case PlayerFacing.East:
                a = Animations.ATTACK_E;
                break;
            case PlayerFacing.South:
                a = Animations.ATTACK_S;
                break;
            case PlayerFacing.West:
                a = Animations.ATTACK_W;
                break;
        }
        Play(a, isLocked, bypassLock);
    }

}
