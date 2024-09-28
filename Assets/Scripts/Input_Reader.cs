using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Input_Reader : MonoBehaviour
{
    // ----------------------------------------
    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected Rigidbody rbody;

    [SerializeField]
    protected string idleAnim = "idle";
    // ----------------------------------------
    delegate void OnInputPressed(Animator anim, ref List<int> currentKeysPressed, ref List<int> currentMotionKeysPressed);
    OnInputPressed queryInputs = null;
    // ----------------------------------------
    [SerializeField]
    Player_Character player = null;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    Transform groundCheckTransform;

    [SerializeField]
    float groundCheckRadius = 0.2f;
    // ----------------------------------------
    [SerializeField]
    private float maxInputDelay = 0.0f;

    [SerializeField]
    Input_Behaviour chargingBehaviour;

    [SerializeField]
    List<Input_Behaviour> groundInputBehaviourList;

    [SerializeField]
    List<Input_Behaviour> groundAtkInputBehaviourList;

    [SerializeField]
    List<Input_Behaviour> aerialInputBehaviourList;

    [SerializeField]
    List<Input_Behaviour> aerialAtkInputBehaviourList;

    [SerializeField]
    List<Input_Behaviour> inputBehaviourList;
    // ----------------------------------------
    [SerializeField]
    private List<int> inputHistory = new List<int>();

    [SerializeField]
    private List<int> inputsPressedOnFrame = new List<int>();

    [SerializeField]
    private List<int> motionInputsPressedOnFrame = new List<int>();

    [SerializeField]
    private InputMapper lastInputValue = InputMapper.NOINPUT;

    [SerializeField]
    private float timeSinceLastInput = 0.0f;
    // ----------------------------------------
    [SerializeField]
    protected bool isGrounded = false;

    [SerializeField]
    protected bool isAttacking = false;

    [SerializeField]
    protected bool isCharging = false;
    // ----------------------------------------
    void Awake()
    {
        inputBehaviourList = new List<Input_Behaviour>();

        inputBehaviourList.AddRange(groundInputBehaviourList);
        inputBehaviourList.AddRange(aerialInputBehaviourList);

        inputBehaviourList.AddRange(groundAtkInputBehaviourList);
        inputBehaviourList.AddRange(aerialAtkInputBehaviourList);
        inputBehaviourList.Add(chargingBehaviour);

        anim = GetComponent<Animator>();

        if (anim)
            anim.Play(idleAnim);

        UpdateGroundedState(isGrounded);
    }

    void OnEnable()
    {
        foreach (Input_Behaviour inputBehaviour in inputBehaviourList)
            queryInputs += inputBehaviour.onKeyPressed;
    }

    void OnDisable()
    {
        foreach (Input_Behaviour inputBehaviour in inputBehaviourList)
            queryInputs -= inputBehaviour.onKeyPressed;
    }
    // ----------------------------------------
    // Update is called once per frame
    void Update()
    {
        if (!player || queryInputs == null)
            return;
        queryInputs(anim, ref inputsPressedOnFrame, ref motionInputsPressedOnFrame);

        // User did not press a single key during this frame
        if (inputsPressedOnFrame.Count <= 0)
        {
            timeSinceLastInput += Time.deltaTime;
            if (timeSinceLastInput > maxInputDelay)
            {
                timeSinceLastInput = 0.0f;

                if (inputHistory.Count > 0)
                {
                    inputHistory.Clear();
                    lastInputValue = InputMapper.NOINPUT;
                }
            }
        }
        // User pressed at least one key during this frame
        // Record all new key pressed
        else
        {
            inputHistory.AddRange(inputsPressedOnFrame);
            timeSinceLastInput = 0.0f;
        }

        // No Inputs Pressed -> Idle or Falling down, depends on isGrounded
        if (motionInputsPressedOnFrame.Count == 0)
        {
            if (isGrounded && Mathf.Abs(rbody.velocity.y) <= 0.01f && !isAttacking)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleAnim))
                {
                    anim.Play(idleAnim);
                }
            }
            else
            {
                // TODO: Falling Idle
            }
        }

        if (inputHistory.Count > 0)
            lastInputValue = (InputMapper)inputHistory[inputHistory.Count - 1];

        // Reset
        inputsPressedOnFrame.Clear();
        motionInputsPressedOnFrame.Clear();
    }

    void FixedUpdate()
    {
        CheckIfGrounded();
    }
    // ----------------------------------------
    #region Input State Controls - How Repetitive? Yes.
    // Aerial
    public void DisableAerial()
    {
        DisableAerialInputs();
        DisableAerialInputAnims();
    }

    public void EnableAerial()
    {
        EnableAerialInputs();
        EnableAerialInputAnims();

        //EnableAerialAtkInputs();
        //EnableAerialAtkInputAnims();
    }

    public void DisableAerialInputs()
    {
        foreach (Input_Behaviour inputBehaviour in aerialInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = false;
    }

    public void DisableAerialAtkInputs()
    {
        foreach (Input_Behaviour inputBehaviour in aerialAtkInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = false;
    }

    public void EnableAerialInputs()
    {
        foreach (Input_Behaviour inputBehaviour in aerialInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = true;
    }

    public void EnableAerialAtkInputs()
    {
        foreach (Input_Behaviour inputBehaviour in aerialAtkInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = true;
    }

    public void DisableAerialInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in aerialInputBehaviourList)
            inputBehaviour.isAnimEnabled = false;
    }

    public void DisableAerialAtkInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in aerialAtkInputBehaviourList)
            inputBehaviour.isAnimEnabled = false;
    }

    public void EnableAerialInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in aerialInputBehaviourList)
            inputBehaviour.isAnimEnabled = true;
    }

    public void EnableAerialAtkInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in aerialAtkInputBehaviourList)
            inputBehaviour.isAnimEnabled = true;
    }

    // Ground
    public void DisableGround()
    {
        DisableGroundInputs();
        DisableGroundInputAnims();

        DisableGroundAtkInputs();
        DisableGroundAtkInputAnims();
    }

    public void EnableGround()
    {
        EnableGroundInputs();
        EnableGroundInputAnims();

        EnableGroundAtkInputs();
        EnableGroundAtkInputAnims();
    }

    public void DisableGroundInputs()
    {
        foreach (Input_Behaviour inputBehaviour in groundInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = false;
    }

    public void DisableGroundAtkInputs()
    {
        foreach (Input_Behaviour inputBehaviour in groundAtkInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = false;
    }

    public void EnableGroundInputs()
    {
        foreach (Input_Behaviour inputBehaviour in groundInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = true;
    }

    public void EnableGroundAtkInputs()
    {
        foreach (Input_Behaviour inputBehaviour in groundAtkInputBehaviourList)
            inputBehaviour.isBehaviourEnabled = true;
    }

    public void DisableGroundInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in groundInputBehaviourList)
            inputBehaviour.isAnimEnabled = false;
    }

    public void DisableGroundAtkInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in groundAtkInputBehaviourList)
            inputBehaviour.isAnimEnabled = false;
    }

    public void EnableGroundInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in groundInputBehaviourList)
            inputBehaviour.isAnimEnabled = true;
    }

    public void EnableGroundAtkInputAnims()
    {
        foreach (Input_Behaviour inputBehaviour in groundAtkInputBehaviourList)
            inputBehaviour.isAnimEnabled = true;
    }

    // Charging
    public void DisableChargeInput()
    {
        chargingBehaviour.isBehaviourEnabled = false;
    }

    public void EnableChargeInput()
    {
        chargingBehaviour.isBehaviourEnabled = true;
    }
    #endregion
    // ----------------------------------------
    private void CheckIfGrounded()
    {
        Collider[] hitColliders = Physics.OverlapSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);
        bool grounded = hitColliders.Length > 0;

        // Debug
        //Debug.Log("Touched Ground?: " + hitColliders.Length);

        if (grounded != isGrounded)
            UpdateGroundedState(grounded);
    }

    private void UpdateGroundedState(bool newState)
    {
        if (isGrounded != newState)
            isGrounded = newState;

        // Debug
        //Debug.Log("Updating is Grounded State: " + isGrounded);

        // If we are attacking, enable the following:
        // N/A (Disable All Other Inputs)
        // 
        // Disable the following:
        // 1. Walking (Hor Movement)
        // 2. Jumping
        // 3. Attacking (Ground)
        // 5. Blocking
        // 6. Attacking (Aerial)
        // 7. Walking (Hor Movement)
        //
        // Maintain:
        // 1. Charging
        if (isCharging)
        {
            DisableGround();
            DisableAerial();
        }
        else
        {
            // If we are not attacking
            if (!isAttacking)
            {
                // If we are on the ground, enable the following:
                // 1. Walking (Hor Movement)
                // 2. Jumping
                // 3. Attacking (Ground)
                // 4. Charging
                // 5. Blocking
                // 
                // Disable the following:
                // 1. Aerial Attacking
                if (isGrounded)
                {
                    EnableGroundInputs();
                    EnableGroundAtkInputs();

                    EnableGroundInputAnims();

                    EnableAerialInputAnims();
                    EnableAerialInputs();

                    EnableChargeInput();
                }
                // If we are falling/jumping, enable the following:
                // 1. Walking (Hor Movement)
                // 2. Attacking (Aerial)
                // 
                // Disable the following:
                // 1. Jumping
                // 2. Charging
                // 3. Blocking
                // 4. Attacking (Ground)
                else
                {
                    EnableGroundInputs();
                    DisableGroundInputAnims();

                    DisableAerialInputAnims();
                    DisableAerialInputs();

                    DisableChargeInput();
                }
            }
            // If we are attacking, enable the following:
            // N/A (Disable All Other Inputs)
            // 
            // Disable the following:
            // 1. Walking (Hor Movement)
            // 2. Jumping
            // 3. Attacking (Ground)
            // 4. Charging
            // 5. Blocking
            // 6. Attacking (Aerial)
            // 7. Walking (Hor Movement)
            else
            {
                DisableGround();
                DisableAerial();
                DisableChargeInput();
            }
        }
    }
    // ----------------------------------------
    public void StartAttack(string attackName)
    {
        isAttacking = true;
        UpdateGroundedState(isGrounded);
        //switch (attackName)
        //{
        //    case "primary_ground":
        //        break;
        //    case "secondary_ground":
        //        break;
        //}
    }

    public void StopAttack(string attackName)
    {
        isAttacking = false;
        UpdateGroundedState(isGrounded);
        switch (attackName)
        {
            case "primary_ground":
                Ground_Primary_Attack_Behaviour beh1 = GetComponentInChildren<Ground_Primary_Attack_Behaviour>();
                if (beh1.isPressed)
                    beh1.isPressed = false;
                break;
            case "secondary_ground":
                Ground_Secondary_Attack_Behaviour beh2 = GetComponentInChildren<Ground_Secondary_Attack_Behaviour>();
                if (beh2.isPressed)
                    beh2.isPressed = false;
                break;
            case "charge":
                Charge_Behaviour beh3 = GetComponentInChildren<Charge_Behaviour>();
                if (beh3.isPressed)
                    beh3.isPressed = false;
                break;
        }
    }
    // ----------------------------------------
    public void StartCharging()
    {
        isCharging = true;
        UpdateGroundedState(isGrounded);
    }

    public void StopCharging()
    {
        isCharging = false;
        UpdateGroundedState(isGrounded);
    }
    // ----------------------------------------
}
