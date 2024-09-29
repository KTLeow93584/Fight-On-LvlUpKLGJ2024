using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horizontal_Movement_Behaviour : Input_Behaviour
{
    // ----------------------------------------
    // targetAnimName = Run
    // ----------------------------------------
    [SerializeField]
    private Transform targetTransform = null;

    // Movement variables
    [SerializeField]
    private float moveSpeed = 8f;

    private bool isRunningLeft;
    private bool isRunningRight;
    // ----------------------------------------
    [SerializeField]
    private Input_Reader localReaderScript = null;
    // ----------------------------------------
    // For the sake of game jam schedule, couply linked dependencies EVERYWHERE! Because why not.
    [SerializeField]
    Input_Behaviour jumpBehaviour = null;
    // ----------------------------------------
    protected override void Update()
    {
        if (!isBehaviourEnabled)
        {
            StopMovement();
            return;
        }

        if (localReaderScript.IsBlocking())
        {
            StopMovement();
            return;
        }

        float moveForce = (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum));

        if (moveForce < 0)
        {
            isRunningLeft = true;
            isRunningRight = false;
        }
        else if (moveForce > 0)
        {
            isRunningRight = true;
            isRunningLeft = false;
        }

        if (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) == 0)
            StopMovement();

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (localReaderScript.IsBlocking())
        {
            rbody.velocity = new Vector2(0.0f, rbody.velocity.y);
            return;
        }

        // Non-Kinematic Movement
        if (isRunningLeft)
            rbody.velocity = new Vector2(-moveSpeed, rbody.velocity.y);
        else if (isRunningRight)
            rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
    }

    public override void onKeyPressed(Animator anim, ref List<int> currentKeysPressed, ref List<int> currentMotionKeysPressed)
    {
        InputMapper mappedInput = InputMapper.UP;

        if (!isBehaviourEnabled)
            return;

        if (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) > 0)
        {
            if (!isPressed)
            {
                isPressed = true;

                mappedInput = InputMapper.RIGHT;

                if (currentKeysPressed.Count <= 0 || (currentKeysPressed.Count > 0 && currentKeysPressed[currentKeysPressed.Count - 1] != (int)mappedInput))
                    currentKeysPressed.Add((int)mappedInput);
            }

            ProcessMotionInput(anim);

            currentMotionKeysPressed.Add((int)mappedInput);
        }
        else if (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) < 0)
        {
            if (!isPressed)
            {
                isPressed = true;

                mappedInput = InputMapper.LEFT;

                if (currentKeysPressed.Count <= 0 || (currentKeysPressed.Count > 0 && currentKeysPressed[currentKeysPressed.Count - 1] != (int)mappedInput))
                    currentKeysPressed.Add((int)mappedInput);
            }

            ProcessMotionInput(anim);
            currentMotionKeysPressed.Add((int)mappedInput);
        }
        else
        {
            if (isPressed)
                isPressed = false;
        }
    }

    private void ProcessMotionInput(Animator anim)
    {
        if ((!jumpBehaviour || jumpBehaviour && !jumpBehaviour.isPressed) && !localReaderScript.IsBlocking())
        {
            if (isAnimEnabled)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(targetAnimName))
                    anim.Play(targetAnimName);
            }
        }
    }
    // ----------------------------------------
    private void StopMovement()
    {
        if (isRunningLeft || isRunningRight)
            rbody.velocity = new Vector2(0, rbody.velocity.y);

        isRunningLeft = false;
        isRunningRight = false;
    }
    // ----------------------------------------
    private float GetPlayerDirection()
    {
        return targetTransform.localScale.x;
    }

    private void FlipSprite()
    {
        if (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) < 0 && rbody.velocity.x < 0)
        {
            targetTransform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) > 0 && rbody.velocity.x > 0)
        {
            targetTransform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    // ----------------------------------------
}
