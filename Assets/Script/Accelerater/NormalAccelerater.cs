using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class NormalAccelerater : Accelerater
{
    public NormalAccelerater(Player player,
                             float x_accelarate,
                             float gravity,
                             float jump_accelerate,
                             float x_speed_decrease_per_unit,
                             float y_speed_decrease_per_unit,
                             float run_additional_x_speed,
                             float run_additional_y_speed,
                             float aim_additional_x_speed,
                             float aim_additional_y_speed) 
    : base(player,
           x_accelarate,
           gravity,
           jump_accelerate,
           x_speed_decrease_per_unit,
           y_speed_decrease_per_unit,
           run_additional_x_speed,
           run_additional_y_speed,
           aim_additional_x_speed,
           aim_additional_y_speed)
    {
    }

    public override void x_accelerate(int inputDirection)
    {
        if (target.getMoveDirection() != inputDirection)
        {
            this.apply_x_Decrease();
        }
        if (target.x_speed_biggerThan(target.get_max_x_speed()))
        {
            this.apply_x_Decrease();
            return;
        }
        target.x_speed = target.x_speed + this.x_accelarate * inputDirection * Time.deltaTime;

        this.solveSpeedOverflow("X");
    }
    public override void jump()
    {
        target.y_speed += jump_accelerate;
        //if (target.isOnGround)
        //{
        //    target.isOnGround = false;
        //}
        this.solveSpeedOverflow("Y");
    }
    public override void applyGravity()
    {
        target.y_speed -= this.gravity*Time.deltaTime;

        if (target.y_speed < 0)
        {
            //下降的时候，重力影响加大
            target.isFalling = true;
            target.y_speed -= 0.5f * this.gravity * Time.deltaTime;
        }
        this.solveSpeedOverflow("Y");
    }
    protected override void solveSpeedOverflow(string type)
    {
        if (target.is_x_speed_overflow() && (type == "x" || type == "X" || type == "xY" || type == "Xy"))
        {
            target.x_speed = target.get_max_x_speed() * target.getMoveDirection();
        }
        if (target.is_y_speed_overflow() && (type == "y" || type == "Y" || type == "xY" || type == "Xy"))
        {
            int direction = 1;
            if (target.y_speed < 0) direction = -1;

            target.y_speed = direction * target.get_max_y_speed();
        }
    }
    
    public override void apply_x_Decrease()
    {
        if (this.is_static("x"))
        {
            target.x_speed = 0;
            return;
        }

        int moveDirection = ((Player)target).getMoveDirection();
        if (target is Player && ((Player)target).getInputState(InputState.AIM) == 1)
        {
            target.x_speed -= moveDirection * this.x_speed_decrease_per_unit * Time.deltaTime;
            return;
        }

        target.x_speed -= target.getFaceDirection() * this.x_speed_decrease_per_unit * Time.deltaTime;

        //减速过头了的情况
        if(target.getFaceDirection() != target.getMoveDirection())
        {
            target.x_speed = 0;
        }
       
    }

    public override void applyRunSpeed()
    {
        if(!base.runApplied)
        {
            target.set_max_x_speed(target.get_max_x_speed() + this.run_additional_x_speed);
            base.runApplied = true;
        }
    }
    public override void removeRunSpeed()
    {
        if(base.runApplied)
        {
           target.set_max_x_speed(target.get_max_x_speed() - this.run_additional_x_speed);
           base.runApplied = false;
        }

    }

    public override void applyAimSpeed()
    {
        if (!base.aimApplied)
        {
            target.set_max_x_speed(target.get_max_x_speed() + this.aim_additional_x_speed);
            base.aimApplied = true;
        }
    }
    public override void removeAimSpeed()
    {
        if (base.aimApplied)
        {
            target.set_max_x_speed(target.get_max_x_speed() - this.aim_additional_x_speed);
            base.aimApplied = false;
        }
    }
}
