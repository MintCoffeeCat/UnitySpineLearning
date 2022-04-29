using UnityEngine;
using System.Collections;
using Spine.Unity;

/*
 * @brief   一切可移动、有血量的物体的基类。包括玩家、敌人、可破坏物、掉落道具等。
 *          抽象基类，需要子类进行实现
 */
abstract public class Target : MonoBehaviour,SpineAnimatable
{
    [SerializeReference] public Accelerater acc;

    [Space]
    [Header("对象数值")]
    [SerializeField] public float x_speed;
    [SerializeField] protected float max_x_speed;
    [SerializeField] public float y_speed;
    [SerializeField] protected float max_y_speed;
    [SerializeField] protected int max_Hp;
    [SerializeField] protected int current_HP;
    [HideInInspector] public Rigidbody2D rb;
    //[ReadOnly] public bool isOnGround = false;
    [ReadOnly] public bool isFalling = false;

    [Space]
    [Header("碰撞检测")]
    [SerializeReference] protected TriggerDetecter detecter;

    [Space]
    [Header("骨骼动画")]
    [ReadOnly]public string animationState;
    [SerializeField] public SkeletonAnimation anim;
    [SerializeField] public AnimationReferenceAsset Idle;
    [SerializeField] public AnimationReferenceAsset Walk;
    [SerializeField] public AnimationReferenceAsset Run;


    /* 
     * @brief   检测X方向的速度是否溢出 
     * @return  无
     */
    public bool is_x_speed_overflow()
    {
        return Mathf.Abs(this.x_speed) > this.max_x_speed;
    }

    /* 
     * @brief   检测Y方向的速度是否溢出 
     * @return  无
     */
    public bool is_y_speed_overflow()
    {
        return Mathf.Abs(this.y_speed) > this.max_y_speed;
    }

    /* 
     * @brief   将当前X方向的速度与 [参数x] 做比较
     * @param   x   待比较的数字
     * @return  bool    速度大于 [参数x] 
     */
    public bool x_speed_biggerThan(float x)
    {
        return Mathf.Abs(this.x_speed) > x;
    }

    /* 
     * @brief   将当前X方向的速度与 [参数x] 做比较
     * @param   x   待比较的数字
     * @return  bool    速度小于 [参数x] 
     */
    public bool x_speed_smallerThan(float x)
    {
        return Mathf.Abs(this.x_speed) < x;
    }

    /* 
     * @brief   设置最大速度 
     * @param   要设置的速度值
     * @return  无
     */
    public void set_max_x_speed(float x)
    {
        this.max_x_speed = x;
    }

    /*
     * @brief   获取当前X方向的最大速度
     * @return  bool    最大速度max_x_speed
     */
    public float get_max_x_speed()
    {
        return this.max_x_speed;
    }

    /*
     * @brief   获取当前Y方向的最大速度
     * @return  float   最大速度max_y_speed
     */
    public float get_max_y_speed()
    {
        return this.max_y_speed;
    }


    /*
     * @brief   获取当前人脸朝向
     * @return  int     人脸朝向
     *                  -1  向左
     *                   1  向右
     */
    public int getFaceDirection()
    {
        return (int)this.transform.localScale.x;
    }

    /*
     * @brief   获取当前移动方向。注意，由于Player以及部分Enemy（继承自Target）拥有Aim状态
     *          因此人脸朝向与移动方向不一定相同
     * @return  int     移动方向
     *                  -1  向左
     *                   1  向右
     */
    public int getMoveDirection()
    {
        if (this.x_speed == 0) return 0;

        return (int)(this.x_speed / Mathf.Abs(this.x_speed));
    }
    /*
 * @brief   判断当前角色是否静止不动
 * @param   type    用于区分检测X方向的静止或者Y方向的静止
 *                  type == "x"     检测X方向
 *                  type == "y"     检测Y方向
 *                  
 * @return  bool    是否静止
 */
    public bool is_static(string type)
    {
        return this.acc.is_static(type);
    }
    protected void speedApplyToVelocity()
    {
        this.rb.velocity = new Vector2(this.x_speed, this.y_speed);
    }
    /*
     * @brief   MonoBehavior进行初始化的操作
     * @return  无
     */
    abstract protected void Start();

    /*
     * @brief   MonoBehavior进行每一帧更新的操作。在当前Target基类中，
     *          先执行物理数据更新，再执行动画更新
     *          
     * @return  无
     */
    virtual protected void Update()
    {
        physicsUpdate();
        speedApplyToVelocity();
        animationUpdate();
    }

    /*
     * @brief   进行物理数据更新的操作，待子类实现
     * @return  无
     */
    abstract protected void physicsUpdate();

    /*
     * @brief   进行动画更新的操作，待子类实现
     * @return  无
     */
    abstract protected void animationUpdate();
    public string getCurrentAnimationName() { return this.animationState; }
    public Spine.AnimationState GetAnimatonState() { return this.anim.state; }

    public void setCurrentANimationName(string name)
    {
        this.animationState = name;
    }
    public bool getDetectState(DetectType type)
    {
        return this.detecter.state.getDetect(type);
    }
}
