using UnityEngine;
using Spine.Unity;
using System;
using Spine;

[Serializable]
public class PlayerAnimationController : IAnimationController
{
    public int MAX_WALK_SPEED = 4;
    private PlayerAnimationSet animationSet;
    private SkeletonAnimation animation;

    public PlayerAnimationController(SkeletonAnimation player, PlayerAnimationSet set):base(set)
    {
        this.animation = player;
        this.animationSet = set;
    }


    public void save_lastFrame()
    {
        this.lastFrameState = animationSet.animationState;
    }

    public void aimAnimation()
    {
        this.setAnimation(animation, animationSet.Aim, false);
    }
    public void aimmingAnimation()
    {
        this.setAnimation(animation, animationSet.Aiming, true);
    }
    public void aimWalkAnimation()
    {
        this.setAnimation(animation, animationSet.AimForwardStep, true);
    }
    public void idleAnimation()
    {
        this.setAnimation(animation, animationSet.Idle, true);
    }

    public void walkAnimation()
    {
        this.setAnimation(animation, animationSet.Walk, true);
    }

    public void runAnimation()
    {
        this.setAnimation(animation, animationSet.Run, true);
    }

    public void jumpAnimation()
    {
        this.setAnimation(animation, animationSet.JumpUp, true);
    }
    public void fallAnimation()
    {
        this.setAnimation(animation, animationSet.Fall, true);
    }
}