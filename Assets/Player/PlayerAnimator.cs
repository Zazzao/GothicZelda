

public class PlayerAnimator : ActorAnimator{


    public void PlayWalk(FacingDirection facing) {
        Play(ActorAnimator.ActorAnimation.Walk, facing, false, false);
    }
    public void PlayIdle(FacingDirection facing) {
        Play(ActorAnimator.ActorAnimation.Idle, facing, false, false);
    }

    public void PlayDying(FacingDirection facing){
        Play(ActorAnimator.ActorAnimation.Dying, ActorAnimator.FacingDirection.South, true, true);
    }

    public void PlayAttack(FacingDirection facing){
        Play(ActorAnimator.ActorAnimation.Attack, facing, true, false);
    }

    public void PlayRoll(FacingDirection facing) {
        Play(ActorAnimator.ActorAnimation.Roll, facing, true, false);
    }


}
