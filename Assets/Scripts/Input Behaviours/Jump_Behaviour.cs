using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_Behaviour : Input_Behaviour
{
    // ----------------------------------------
    // targetAnimName = Jump
    // ----------------------------------------
    // Jumping variables
    [SerializeField]
    private float jumpForce = 15f;

    [SerializeField]
    private float gravitationalPullForce = 10f;
    // ----------------------------------------
    void FixedUpdate()
    {
        if (rbody.velocity.y < 0.0f)
            rbody.AddForce(Vector2.down * gravitationalPullForce);
        else if (rbody.velocity.y > 0.0f)
            rbody.AddForce(Vector2.down * (gravitationalPullForce * 0.5f));
    }

    public override void onKeyPressed(Animator anim, ref List<int> currentKeysPressed, ref List<int> currentMotionKeysPressed)
    {
        InputMapper mappedInput = InputMapper.UP;
        if (isPressed != !isBehaviourEnabled)
            isPressed = !isBehaviourEnabled;

        if (!isBehaviourEnabled)
            return;

        if (Input.GetAxis("Jump_P" + player.assignedPlayerNum) > 0)
        {
            if (!isPressed)
            {
                isPressed = true;
                currentKeysPressed.Add((int)mappedInput);

                if (isAnimEnabled && !anim.GetCurrentAnimatorStateInfo(0).IsName(targetAnimName))
                    anim.Play(targetAnimName);

                Jump();
            }

            isBehaviourEnabled = false;
        }
    }
    // ----------------------------------------
    private void Jump()
    {
        rbody.velocity = new Vector2(rbody.velocity.x, jumpForce);
        //anim.Play(targetAnim);
    }
    // ----------------------------------------
}
