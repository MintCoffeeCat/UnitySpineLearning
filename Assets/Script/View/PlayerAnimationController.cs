using UnityEngine;
using Spine.Unity;
using System;
using Spine;

[Serializable]
public class PlayerAnimationController:IAnimationController
{
    public int MAX_WALK_SPEED = 4;
    private Player player;


    public PlayerAnimationController(Player player)
    {
        this.player = player;
    }

    private void check_move_state()
    {
        if(player.is_static("x") && player.is_static("y"))
        {
            if(player.animationState.Equals("FallOnGround"))
            {
                this.addAnimation(player, player.Idle, true);
            }
            else
            {
                this.setAnimation(player, player.Idle, true);
            }

        }
        if(player.x_speed_biggerThan(0) && !player.x_speed_biggerThan(MAX_WALK_SPEED))
        {
            this.setAnimation(player, player.Walk, true);
        }
        else if(player.x_speed_biggerThan(MAX_WALK_SPEED))
        {
            this.setAnimation(player, player.Run, true);
        }
    }
    
    public void check_aim_state()
    {
        if(player.getInputState(InputState.AIM)!=1)
        {
            this.check_move_state();
            return;
        }
        if (player.is_static("x"))
        {
            if (player.animationState.Equals("Aim") || player.animationState.Equals("Aiming")) return;

            TrackEntry entry = this.setAnimation(player, player.Aim, false);
            if (entry != null)
            {
                entry = player.anim.state.AddAnimation(0, player.Aiming, true, 0);
                entry.Start += this.updateState;
            }
        }
        else 
        {
            this.setAnimation(player, player.AimForwardStep, true);
        }
    }

    public void check_jump_state()
    {
        if(player.getInputState(InputState.JUMP) == 1)
        {
            this.setAnimation(player, player.JumpUp, false);
        }
    }
    public void check_fall_state()
    {
        if (player.isFalling)
        {
            this.setAnimation(player, player.Fall, false);
        }
        else if(this.lastFrameState.Equals("Fall"))
        {
            this.setAnimation(player, player.FallOnGround, false);
        }
    }
    public void save_lastFrame()
    {
        this.lastFrameState = player.animationState;
    }
    public void updateState(TrackEntry entry)
    {
        this.player.animationState = entry.Animation.Name;
    }
}