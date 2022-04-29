using UnityEngine;
using Spine.Unity;
using System;
using Spine;

[Serializable]
public class IAnimationController
{

    protected string lastFrameState;
    protected string lastAdded = "";

   
    public virtual void save_lastFrame(string state)
    {
        this.lastFrameState = state;
    }
    public TrackEntry setAnimation(SpineAnimatable target, AnimationReferenceAsset animation, bool loop, float timeScale = 1)
    {
        if (!animation.name.Equals(target.getCurrentAnimationName()))
        {
            TrackEntry entry = target.GetAnimatonState().SetAnimation(0, animation, loop);
            target.setCurrentANimationName(animation.name);
            entry.TimeScale = timeScale;
            return entry;
        }
        return null;
    }
    public void addAnimation(SpineAnimatable target, AnimationReferenceAsset animation, bool loop, float timeScale = 1)
    {
        if (this.lastAdded.Equals(animation.name)) return;

        TrackEntry entry = target.GetAnimatonState().AddAnimation(0, animation, loop, 0);
        entry.Start += (ent) => {
            target.setCurrentANimationName(ent.Animation.Name);
        };
        this.lastAdded = animation.name;
    }
}