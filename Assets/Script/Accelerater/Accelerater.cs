using UnityEngine;
using System.Collections;
using System;

/*
 * @brief   用来控制对象各个方向上的速度、加速度、阻力等数据
 *          同时也负责定义不同的移动状态下的速度方案
 *          抽象基类，需要子类实现
 */
[Serializable]
public abstract class Accelerater
{
    [ReadOnly] protected Target target;
    [SerializeField] protected float x_accelarate;
    [SerializeField] protected float gravity;
    [SerializeField] protected float jump_accelerate;
    [SerializeField] protected float x_speed_decrease_per_unit;
    [SerializeField] protected float y_speed_decrease_per_unit;
    [SerializeField] protected float run_additional_y_speed;
    [SerializeField] protected float run_additional_x_speed;
    [SerializeField] protected float aim_additional_y_speed;
    [SerializeField] protected float aim_additional_x_speed;
    [SerializeField] public static float StaticJudgementAmount = 0.1f;

    [HideInInspector] public bool runApplied = false;
    [HideInInspector] public bool aimApplied = false;

    public Accelerater(Target target,
                    float x_accelarate,
                     float gravity,
                     float jump_accelerate,
                     float x_speed_decrease_per_unit,
                     float y_speed_decrease_per_unit,
                     float run_additional_x_speed,
                     float run_additional_y_speed,
                     float aim_additional_x_speed,
                     float aim_additional_y_speed)
    {
        this.target = target;
        this.x_accelarate = x_accelarate;
        this.gravity = gravity;
        this.jump_accelerate = jump_accelerate;
        this.x_speed_decrease_per_unit = x_speed_decrease_per_unit;
        this.y_speed_decrease_per_unit = y_speed_decrease_per_unit;
        this.run_additional_y_speed = run_additional_y_speed;
        this.run_additional_x_speed = run_additional_x_speed;
        this.aim_additional_y_speed = aim_additional_y_speed;
        this.aim_additional_x_speed = aim_additional_x_speed;
    }

    /* 实现在X方向上的加速 */
    public abstract void x_accelerate(int inputDirection);
    
    /* 实现跳跃*/
    public abstract void jump();

    /* 实现重力的影响*/
    public abstract void applyGravity();

    /* 实现对速度超出上限时的处理*/
    protected abstract void solveSpeedOverflow(string s);

    /* 实现无输入时，X方向上速度的递减 */
    public abstract void apply_x_Decrease();

    /* 实现跑步输入时，对速度的处理 */
    public abstract void applyRunSpeed();

    /* 实现跑步输入取消时，对速度的处理 */
    public abstract void removeRunSpeed();

    /* 实现瞄准输入时，对速度的处理 */
    public abstract void applyAimSpeed();

    /* 实现瞄准输入取消时，对速度的处理 */
    public abstract void removeAimSpeed();

    /*
     * @brief   判断当前加速器所属的角色是否静止不动
     * @param   type    用于区分检测X方向的静止或者Y方向的静止
     *                  type == "x"     检测X方向
     *                  type == "y"     检测Y方向
     *                  
     * @return  bool    是否静止
     */
    public bool is_static(string type)
    {
        if (type.Equals("x") || type.Equals("X"))
        {
            return Mathf.Abs(this.target.x_speed) < Accelerater.StaticJudgementAmount;
        }
        if (type.Equals("y") || type.Equals("Y"))
        {
            return Mathf.Abs(this.target.y_speed) < Accelerater.StaticJudgementAmount;
        }

        return false;
    }
}
