using UnityEngine;
using System.Collections;

public delegate void commonSolver();
public delegate void directionSolver(int dir);

/*
 * @brief   用来处理对象行为的工具类。
 *          主要负责根据移动状态、输入情况等条件，选择合适的行为
 *          不参与具体行为的细节处理
 */
public class MovementSolver 
{
    public static void xMovement(PlayerController controller, directionSolver solveInput, commonSolver solveNoInput)
    {
        if (controller.xAxis != 0) solveInput((int)controller.xAxis);
        else solveNoInput();
    }
    public static void xMovement(PlayerController controller, Accelerater acc)
    {
        if (controller.xAxis != 0) acc.x_accelerate((int)controller.xAxis);
        else acc.apply_x_Decrease();
    }

    public static void solveJump(Player player, commonSolver solveInput)
    {
        if(player.getInputState(InputState.JUMP) == 1 && player.getDetectState(DetectType.ON_GROUND))
        {
            solveInput();
        }
    }
    public static void solveJump(Player player, Accelerater acc)
    {
        if (player.getInputState(InputState.JUMP) == 1 && player.getDetectState(DetectType.ON_GROUND))
        {
            acc.jump();
        }
    }

    public static void solveGravity(Target target, commonSolver solveGravity)
    {
        if (target.getDetectState(DetectType.ON_GROUND)) return;

        solveGravity();
    }
    public static void solveGravity(Target target, Accelerater acc)
    {
        if (target.getDetectState(DetectType.ON_GROUND)) return;

        acc.applyGravity();
    }

    public static void solveFaceDirection(Target target, Accelerater acc)
    {
        if (target is Player && ((Player)target).getInputState(InputState.AIM) != 0) return;
        
        if (!acc.is_static("x"))
        {
            int newfaceDirection = target.getMoveDirection();
            target.transform.localScale = new Vector3(newfaceDirection, 1, 1);
        }
    }

    public static void solveRun(PlayerController controller, commonSolver solveInput, commonSolver solveNoInput)
    {
        if (controller.validRunButtonDown)
        {
            solveInput();
        }
        else solveNoInput();
    }
    public static void solveRun(PlayerController controller, Accelerater acc)
    {
        if (controller.validRunButtonDown)
        {
            acc.applyRunSpeed();
        }
        else acc.removeRunSpeed();
    }

    public static void solveAim(PlayerController controller, commonSolver solveInput, commonSolver solveNoInput)
    {
        if (controller.validAimButtonDown)
        {
            solveInput();
        }
        else solveNoInput();
    }
    public static void solveAim(PlayerController controller, Accelerater acc)
    {
        if (controller.validAimButtonDown)
        {
            acc.applyAimSpeed();
        }
        else acc.removeAimSpeed();
    }
}
