using UnityEngine;
using Spine.Unity;
using System;
using Spine;

[Serializable]
public class IAnimationController
{

    protected string lastFrameState;
    protected string lastAdded = "";
    private SpineAnimationSet animationSet;
   
    public IAnimationController(SpineAnimationSet st)
    {
        this.animationSet = st;
    }

    public virtual void save_lastFrame(string state)
    {
        this.lastFrameState = state;
    }
    public TrackEntry setAnimation(SkeletonAnimation target, AnimationReferenceAsset animation, bool loop, float timeScale = 1)
    {
        if (!animation.name.Equals(target.AnimationName))
        {
            TrackEntry entry = target.AnimationState.SetAnimation(0, animation, loop);
            animationSet.animationState = animation.name;
            entry.TimeScale = timeScale;
            return entry;
        }
        return null;
    }
    public void addAnimation(SkeletonAnimation target, AnimationReferenceAsset animation, bool loop, float timeScale = 1)
    {
        if (this.lastAdded.Equals(animation.name)) return;

        TrackEntry entry = target.AnimationState.AddAnimation(0, animation, loop, 0);
        this.lastAdded = animation.name;
    }
}