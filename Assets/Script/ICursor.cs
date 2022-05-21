using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICursor : MonoBehaviour,SpineAnimatable
{
    public string animationState = "";
    public SkeletonAnimation anim;
    private CursorAnimationController animationController;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        Cursor.visible = false;

        CursorAnimationSet cursorSet = this.GetComponent<CursorAnimationSet>();
        if (cursorSet)
            animationController = new CursorAnimationController(anim, cursorSet);
        else
            Debug.LogError("光标指针对象没有设置光标动画集C#脚本：CursorAnimationSet");
    }

    private void Awake()
    {
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        this.checkFocus();
        this.updatePosition();
    }

    private void updatePosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0;
        this.transform.position = mouse;
    }
    private void checkFocus()
    {
        if (Input.GetButtonDown("Aim"))
        {
            this.animationController.aimAnimation();
            Debug.Log("Aim click");
        }
        else if(Input.GetButtonUp("Aim"))
            this.animationController.idleAnimation();
    }
    string SpineAnimatable.getCurrentAnimationName()
    {
        return this.animationState;
    }

    Spine.AnimationState SpineAnimatable.GetAnimatonState()
    {
        return this.anim.state;
    }

    public void setCurrentANimationName(string name)
    {
        this.animationState = name;
    }
}
