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
    [Header("�������")]
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
        //����ƶ��İ�������
        inputController.check_x_axis();

        //��Ⲣ��¼�ܲ�����������
        inputController.check_RunButton();

        //��Ⲣ��¼��Ծ����������
        inputController.check_jumpButton(this);

        //��Ⲣ��¼��׼����������
        //���ȼ�:  ��׼ > �ܲ�, ��׼��ȡ���ܲ�״̬
        inputController.check_AimButton(this);
    }
    override protected void physicsUpdate()
    {
        //����ٶȷ��򡢸�����������
        MovementSolver.solveFaceDirection(this,this.acc);

        //����ܲ�״̬���������ٶ��������
        MovementSolver.solveRun(this.inputController, this.acc);

        //�����׼״̬���������ٶ��������
        MovementSolver.solveAim(this.inputController, this.acc);

        //������Ծ
        MovementSolver.solveJump(this, this.acc);

        //����������Y�����ϵ�Ӱ��
        MovementSolver.solveGravity(this, this.acc);

        //����X������ƶ��Ĵ���
        MovementSolver.xMovement(this.inputController, this.acc);


    }
    override protected void animationUpdate()
    {
        //�������á���·���ܲ��Ķ���
        //������׼�Ķ���
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