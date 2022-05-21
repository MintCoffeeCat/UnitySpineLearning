using UnityEngine;
using System.Collections;
using Spine.Unity;

public class CursorAnimationController : IAnimationController
{

    private CursorAnimationSet animationSet;
    private SkeletonAnimation animation;
    public CursorAnimationController(SkeletonAnimation ani,CursorAnimationSet aniSet) : base(aniSet)
    {
        this.animation = ani;
        this.animationSet = aniSet;
    }
    public void aimAnimation() 
    {
        this.setAnimation(animation, animationSet.focus, false);
    }

    public void idleAnimation()
    {
        this.setAnimation(animation, animationSet.unfocus, false);
    }
}
