using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
     public AnimationReferenceAsset Aim;
     public AnimationReferenceAsset Aiming;
     public AnimationReferenceAsset AimForwardStep;
     public AnimationReferenceAsset JumpUp;
     public AnimationReferenceAsset Fall;
     public AnimationReferenceAsset FallOnGround;


    [Space]
    [Header("控制相关")]
    [SerializeField] protected PlayerController inputController;
    [SerializeField] protected PlayerAnimationController animationController;


    // Start is called before the first frame update
    override protected void Start()
    {
        base.acc = new NormalAccelerater(this,8,30,15,7,0,2,0,-1,0);
        base.rb = this.GetComponent<Rigidbody2D>();
        BoxCollider2D footD=null, headD=null, faceD=null;

        BoxCollider2D[] coll = this.gameObject.GetComponentsInChildren<BoxCollider2D>();
        foreach(BoxCollider2D box in coll)
        {
            if(box.gameObject.name.Equals("FootDetect"))
            {
                footD = box;
            }else if (box.gameObject.name.Equals("HeadDetect"))
            {
                headD = box;
            }else if(box.gameObject.name.Equals("BodyFaceDetect"))
            {
                faceD = box;
            }
        }

        base.detecter = new PlayerTriggerDetecter(faceD,footD,headD,null,null,null,null,null,null);
        base.detecter.ground = LayerMask.GetMask("Ground");
        ((PlayerTriggerDetecter)base.detecter).addDelegates(
            () => { return this.GetComponent<CapsuleCollider2D>().IsTouchingLayers(detecter.ground); },
            () => { this.isFalling = false;
                    this.y_speed = 0;},
            () => { return this.GetComponent<CapsuleCollider2D>().IsTouchingLayers(detecter.ground); },
            () => {
                this.isFalling = true;
                this.y_speed = 0;
            },
            
            ()=> {

                bool body = this.GetComponent<CapsuleCollider2D>().IsTouchingLayers(detecter.ground);
                int faceDire = this.getFaceDirection();

                if (faceDire == 0) return false;
                bool sameDirection = this.getInputState(InputState.X) == faceDire;

                return body && sameDirection;
            },
            () => { this.x_speed = 0; }
            );

        this.GetComponent<MeshRenderer>().sortingLayerName = "target";

        //this.isOnGround = false;
        this.inputController = new PlayerController();
        this.animationController = new PlayerAnimationController(this);
    }

    // Update is called once per frame
    override protected void Update()
    {
        InputDetect();
        physicsUpdate();
        detecter.Update();
        animationUpdate();
        base.speedApplyToVelocity();


    }
    public void follow_mouse(ISkeletonAnimation animated)
    {
        inputController.gun_follow_mouse(this);
    }
    protected void InputDetect()
    {
        //检测移动的按键输入
        inputController.check_x_axis();

        //检测并记录跑步按键的输入
        inputController.check_RunButton();

        //检测并记录跳跃按键的输入
        inputController.check_jumpButton(this);

        //检测并记录瞄准按键的输入
        //优先级:  瞄准 > 跑步, 瞄准会取消跑步状态
        inputController.check_AimButton(this);
    }
    override protected void physicsUpdate()
    {
        //检测速度方向、更改人脸方向
        MovementSolver.solveFaceDirection(this,this.acc);

        //检测跑步状态，并更改速度相关属性
        MovementSolver.solveRun(this.inputController, this.acc);

        //检测瞄准状态，并更改速度相关属性
        MovementSolver.solveAim(this.inputController, this.acc);

        //进行跳跃
        MovementSolver.solveJump(this, this.acc);

        //进行重力对Y方向上的影响
        MovementSolver.solveGravity(this, this.acc);

        //进行X方向的移动的处理
        MovementSolver.xMovement(this.inputController, this.acc);


    }
    override protected void animationUpdate()
    {
        //设置闲置、走路、跑步的动画
        //设置瞄准的动画
        animationController.check_aim_state();

        animationController.check_jump_state();

        animationController.check_fall_state();

        animationController.save_lastFrame();
    }

    public int getInputState(InputState state)
    {
        return this.inputController.InputDict[state]();
    }
    

}


class PlayerDetectStateManager
{

}