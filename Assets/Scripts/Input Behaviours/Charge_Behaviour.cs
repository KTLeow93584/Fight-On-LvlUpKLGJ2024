using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge_Behaviour : Input_Behaviour
{
    // ----------------------------------------
    // targetAnimName = N/A (Default back to Idle) -> No New Animation, Just Spawn simple particle system around player.
    // ----------------------------------------
    // Jumping variables
    [SerializeField]
    private float chargeRate = 25f;

    [SerializeField]
    private ParticleSystem particle = null;

    [SerializeField]
    private Energy_Component energyComponent = null;

    [SerializeField]
    private Input_Reader inputReaderComponent = null;

    [SerializeField]
    private float minHoldDuration = 0.0f;
    private float holdDuration = 0.0f;
    // ----------------------------------------
    public override void onKeyPressed(Animator anim, ref List<int> currentKeysPressed, ref List<int> currentMotionKeysPressed)
    {
        InputMapper mappedInput = InputMapper.CHARGE;

        if (!isBehaviourEnabled)
            return;

        if (isPressed)
        {
            if (!particle.isPlaying)
                particle.Play();

            if (energyComponent)
                energyComponent.AddValue(chargeRate * Time.deltaTime);

            if (Input.GetAxis("Charge_P" + player.assignedPlayerNum) == 0)
            {
                holdDuration = 0.0f;

                if (particle.isPlaying)
                    particle.Stop();

                isPressed = false;
                inputReaderComponent.StopCharging();
            }
        }
        else
        {
            if (Input.GetAxis("Charge_P" + player.assignedPlayerNum) > 0)
            {
                holdDuration += Time.deltaTime;

                if (holdDuration >= minHoldDuration)
                {
                    inputReaderComponent.StartCharging();

                    isPressed = true;
                    currentKeysPressed.Add((int)mappedInput);

                    if (isAnimEnabled)
                        anim.Play(targetAnimName);
                }
            }
        }
    }
    // ----------------------------------------
}
