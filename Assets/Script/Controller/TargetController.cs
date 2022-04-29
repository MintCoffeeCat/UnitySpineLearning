using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * @brief   用于枚举当前对象的移动状态
 *          主要为外部函数调用字典查询提供方便
 */
public enum InputState
{
    RUN, AIM, JUMP, X
}

/*
 * @brief   用于检测对象的行为输入
 *          比如玩家以键盘、鼠标等设备进行输入；敌人依靠AI进行输入
 *          抽象基类，需要子类实现
 */
public abstract class TargetController
{
    [ReadOnly] public bool validRunButtonDown = false;
    [ReadOnly] public bool validAimButtonDown = false;
    [ReadOnly] public bool validJumpButtonDown = false;
    [ReadOnly] public float xAxis = 0;

    public delegate int InputStateReturner();
    public Dictionary<InputState, InputStateReturner> InputDict = new Dictionary<InputState, InputStateReturner>();

    public TargetController()
    {
        InputDict.Add(InputState.RUN, () => this.validRunButtonDown ? 1 : 0);
        InputDict.Add(InputState.AIM, () => this.validAimButtonDown ? 1 : 0);
        InputDict.Add(InputState.JUMP, () => this.validJumpButtonDown ? 1 : 0);
        InputDict.Add(InputState.X, () => (int)this.xAxis);
    }

    public abstract void check_x_axis();
    public abstract void check_RunButton();
    public abstract void check_jumpButton(Target t);
    public abstract void check_AimButton(Player target);

}
