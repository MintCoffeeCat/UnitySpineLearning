using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SpineAnimationSet : MonoBehaviour
{
    [Header("骨骼动画")]
    [ReadOnly] public string animationState = "";
    public SkeletonAnimation anim;


    // Use this for initialization
    void Start()
    {
        if(anim == null)
        {
            Debug.Log("SpineAnimationSet对象的成员变量anim未初始化（类型：SkeletonAnimation）");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
