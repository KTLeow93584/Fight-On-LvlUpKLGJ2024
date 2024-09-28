using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Secondary_Attack_Behaviour : Input_Behaviour
{
    // ----------------------------------------
    // targetAnimName = Attack1/Attack2
    // ----------------------------------------
    public override void onKeyPressed(Animator anim, ref List<int> currentKeysPressed, ref List<int> currentMotionKeysPressed)
    {
        InputMapper mappedInput = InputMapper.KICK;

        if (!isBehaviourEnabled)
            return;

        if (Input.GetAxis("Kick_P" + player.assignedPlayerNum) > 0)
        {
            if (!isPressed)
            {
                isPressed = true;
                currentKeysPressed.Add((int)mappedInput);

                if (isAnimEnabled)
                    anim.Play(targetAnimName, -1, 0);
            }
        }
        else if (Input.GetAxis("Kick_P" + player.assignedPlayerNum) == 0)
        {
            if (isPressed)
                isPressed = false;
        }
    }
    // ----------------------------------------
}
