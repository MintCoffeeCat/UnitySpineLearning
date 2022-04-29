using UnityEngine;
using System.Collections.Generic;
using Spine;




/*
 * @brief   用于检测键盘、鼠标等设备的输入操作，并记录这些操作
 *          同时也负责枪械跟随鼠标等特殊操作的实现
 *          不参与实现输入操作导致的Player行为（例如移动、射击）
 */
[System.Serializable]
public class PlayerController : TargetController
{
   
    public PlayerController(){}

    public override void check_x_axis()
    {
        this.xAxis = Input.GetAxisRaw("Horizontal");
    }
    public override void check_RunButton()
    {
        if(Input.GetButton("Run"))
        {
            if (!this.validAimButtonDown)
            {
                this.validRunButtonDown = true;
            }
        }
        else if(Input.GetButtonUp("Run"))
        {
            this.validRunButtonDown = false;
        }
    }
    public override void check_jumpButton(Target target)
    {
        if (Input.GetButton("Jump"))
        {
            if (target.getDetectState(DetectType.ON_GROUND))
            {
                this.validJumpButtonDown = true;
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            this.validJumpButtonDown = false;
        }
    }
    public override void check_AimButton(Player target)
    {
        if (Input.GetButtonDown("Aim"))
        {
            this.validAimButtonDown = true;
            this.validRunButtonDown = false;

            target.anim.UpdateLocal += target.follow_mouse;
        }
        else if (Input.GetButtonUp("Aim"))
        {
            this.validAimButtonDown = false;
            target.anim.UpdateLocal -= target.follow_mouse;
        }
    }

    public void gun_follow_mouse(Player target)
    {
        Bone focus = target.anim.Skeleton.FindBone("focus");
        Bone gun = target.anim.Skeleton.FindBone("gun");
        Vector3 gunPos = new Vector2(gun.WorldX, gun.WorldY);
        gunPos = target.anim.transform.TransformPoint(gunPos);
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0;


        Debug.DrawLine(gunPos, mouse);
        if(mouse.x < target.transform.position.x)
        {
            target.transform.localScale = new Vector3(-1, 1, 0);
        }
        else if(mouse.x > target.transform.position.x)
        {
            target.transform.localScale = new Vector3(1, 1, 0);
        }

        mouse = target.anim.transform.InverseTransformPoint(mouse);

        focus.X = mouse.x;
        focus.Y = mouse.y;
    }
}
