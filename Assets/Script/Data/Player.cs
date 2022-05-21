using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float aimSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float slopeCheckDistance;
    [SerializeField]
    private float maxSlopeAngle;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;
    [SerializeField]
    private CapsuleCollider2D cc;
    [Header("脚部着陆区域偏移量")]
    [SerializeField]
    private float groundCheckOffsetX;
    [SerializeField]
    private float groundCheckOffsetY;
    [SerializeField]
    private float groundCheckOffsetR;

    [Header("头部触顶区域偏移量")]
    [SerializeField]
    private float headCheckOffsetX;
    [SerializeField]
    private float headCheckOffsetY;
    [Header("Spine 骨骼动画")]
    public SkeletonAnimation anim;


    [Space]
    private float xInput;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;

    private float movementSpeed;
    private int facingDirection = 1;
    private PlayerAnimationController animationController;

    [Header("逻辑判断")]
    [SerializeField] [ReadOnly] private bool isGrounded;
    [SerializeField] [ReadOnly] private bool isOnSlope;
    [SerializeField] [ReadOnly] private bool isJumping;
    [SerializeField] [ReadOnly] private bool isFalling;
    [SerializeField] [ReadOnly] private bool canWalkOnSlope;
    [SerializeField] [ReadOnly] private bool canJump;
    [SerializeField] [ReadOnly] private bool runInput;
    [SerializeField] [ReadOnly] private bool aimInput; 

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 slopeNormalPerp;

    private Rigidbody2D rb;
 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        if(this.anim == null)
        {
            Debug.LogError("Player的成员变量anim未设置（类型：SkeletonAnimation）");
        }
        PlayerAnimationSet pAnimationSet = this.gameObject.GetComponent<PlayerAnimationSet>();
        if(pAnimationSet == null)
        {
            Debug.LogError("Player未设置动画集的C#脚本：PlayerAnimationSet");

        }
        else
        {
            this.animationController = new PlayerAnimationController(this.anim, pAnimationSet);
        }

    }

    private void Update()
    {
        CheckInput();     
    }

    private void FixedUpdate()
    {
        CheckGround();
        checkFalling();
        SlopeCheck();
        ApplyMovement();
    }

    private void CheckInput()
    {
        this.xInput = Input.GetAxisRaw("Horizontal");
        this.runInput = Input.GetButton("Run");
        this.aimInput = false;
        if (Input.GetButton("Aim"))
        {
            aimInput = true;
        }
        Debug.ClearDeveloperConsole();
        Debug.Log("xInput: " + xInput + ",\tfacingDirection: " + facingDirection);
        if(xInput != facingDirection && !aimInput && xInput != 0)
        {
            int f = (int)xInput;
            Debug.Log(f);
            if (f != 0)
            {
                Debug.Log("direction:" + f);
                facingDirection = f;
                this.transform.localScale = new Vector3(facingDirection, 1, 1);
                Debug.Log("scale:" + this.transform.localScale);
            }
        }


        if(aimInput)
        {
            this.anim.UpdateLocal += this.gun_follow_mouse;
        }
        else
        {
            this.anim.UpdateLocal -= this.gun_follow_mouse;
            anim.skeleton.SetToSetupPose();
        }


        if(xInput == 0)
        {
            if(aimInput)
            {
                this.animationController.aimAnimation();
            }
            else
            {
                this.animationController.idleAnimation();
            }
        }
        else
        {
            if(aimInput)
            {
                this.animationController.aimWalkAnimation();
            }
            else
            {
                if(runInput)
                {
                    this.animationController.runAnimation();
                }
                else
                {
                    this.animationController.walkAnimation();
                }
            }
        }



        if(runInput && isGrounded && !aimInput)
        {
            this.movementSpeed = runSpeed;
        }
        else
        {
            if (aimInput)
            {
                this.movementSpeed = aimSpeed;
            }

            else if (!runInput)
                this.movementSpeed = walkSpeed;
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

    }
    private Vector2 groundCheckCenter()
    {
        if(cc.direction == CapsuleDirection2D.Vertical)
            return new Vector2(
                groundCheck.position.x + cc.offset.x * facingDirection + groundCheckOffsetX, 
                groundCheck.position.y + cc.offset.y - cc.size.y / 2 + cc.size.x / 2 + groundCheckOffsetY + groundCheckOffsetR);

        return new Vector2( 
            groundCheck.position.x + cc.offset.x - facingDirection * cc.size.x / 2 + facingDirection * cc.size.y / 2 + groundCheckOffsetX - groundCheckOffsetR/2, 
            groundCheck.position.y + cc.offset.y + groundCheckOffsetY);
    }
    private Vector2 headCheckCenter()
    {
        if (cc.direction == CapsuleDirection2D.Vertical)
            return new Vector2(
                groundCheck.position.x + cc.offset.x * facingDirection + headCheckOffsetX,
                groundCheck.position.y + cc.offset.y + cc.size.y / 2 - cc.size.x / 2 + headCheckOffsetY);

        return new Vector2(
            groundCheck.position.x + cc.offset.x + cc.size.x / 2 - cc.size.y / 2 + headCheckOffsetX,
            groundCheck.position.y + cc.offset.y + headCheckOffsetY);
    }
    private float groundCheckRadius()
    {
        if (cc.direction == CapsuleDirection2D.Vertical) return cc.size.x / 2 + groundCheckOffsetR;

        return cc.size.y / 2 + groundCheckOffsetR / 2;
    }

    private float headCheckRadius()
    {

        if (cc.direction == CapsuleDirection2D.Vertical) return cc.size.x / 2;

        return cc.size.y / 2;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckCenter(),groundCheckRadius(), whatIsGround);

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if(isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

    }
    private void checkFalling()
    {
        if(rb.velocity.y < 0.0f)
        {
            this.isFalling = true;
            this.rb.gravityScale = 2f;
        }
        else
        {
            this.isFalling = false;
            this.rb.gravityScale = 1.0f;
        }
    }
    private void SlopeCheck()
    {
        Vector2 checkPos = groundCheckCenter();
        checkPos.Set(checkPos.x, checkPos.y - groundCheckRadius());
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        Debug.DrawLine(checkPos, checkPos - new Vector2(0,slopeCheckDistance),Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }                       

            lastSlopeAngle = slopeDownAngle;
           
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }   

    private void ApplyMovement()
    {
        if (isGrounded && !isOnSlope && !isJumping) //if not on slope
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rb.velocity = newVelocity;
        }
        else if (!isGrounded) //If in air
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }

    }


    protected void animationUpdate()
    {
        //设置闲置、走路、跑步的动画
        //设置瞄准的动画
        //animationController.check_aim_state();

        //animationController.check_jump_state();

        //animationController.check_fall_state();

        //animationController.save_lastFrame();
    }

    private void OnDrawGizmos()
    {

        Vector2 posGround = groundCheckCenter();
        Vector2 posHead = headCheckCenter();

        Gizmos.DrawWireSphere(posGround , groundCheckRadius());
        Gizmos.DrawWireSphere(posHead, headCheckRadius());
    }

    public void gun_follow_mouse(ISkeletonAnimation animated)
    {
        if (!this.aimInput) return;
        
        Bone focus = this.anim.Skeleton.FindBone("focus");
        Bone gun = this.anim.Skeleton.FindBone("gun");
        Vector3 gunPos = new Vector2(gun.WorldX, gun.WorldY);
        gunPos = this.anim.transform.TransformPoint(gunPos);
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0;


        Debug.DrawLine(gunPos, mouse);
        if (mouse.x < this.transform.position.x)
        {
            this.transform.localScale = new Vector3(-1, 1, 0);
            this.facingDirection = -1;
        }
        else if (mouse.x > this.transform.position.x)
        {
            this.transform.localScale = new Vector3(1, 1, 0);
            this.facingDirection = 1;
        }

        mouse =this.anim.transform.InverseTransformPoint(mouse);

        focus.X = mouse.x;
        focus.Y = mouse.y;
    }
}