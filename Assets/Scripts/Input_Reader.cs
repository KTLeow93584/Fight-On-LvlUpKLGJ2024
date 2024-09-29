using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Input_Reader : MonoBehaviour
{
    // ----------------------------------------
    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected Rigidbody rbody;

    [SerializeField]
    protected string idleAnim = "Idle";

    [SerializeField]
    protected string hurtAnim = "Hurt";

    [SerializeField]
    protected string blockAnim = "Block";
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

    [SerializeField]
    public Input_Reader otherPlayerInputReaderScript = null;
    // ----------------------------------------
    [SerializeField]
    protected bool isGrounded = false;

    [SerializeField]
    protected bool isAttacking = false;

    [SerializeField]
    protected bool isHit = false;

    [SerializeField]
    protected bool isCharging = false;

    [SerializeField]
    protected bool isBlocking = false;

    private bool forceIdling = false;
    // ----------------------------------------
    private void Awake()
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

        UpdatePlayerState(isGrounded);
    }

    private void Start()
    {
        // Look for the other player's Input Reader Script.
        if (player)
        {
            string targetName = player.assignedPlayerNum % 2 == 0 ? "_P1" : "_P2";
            Transform[] allTransforms = FindObjectsOfType<Transform>(true);
            Transform otherPlayerTransform = allTransforms.FirstOrDefault(obj => obj.name.EndsWith(targetName) && obj.GetComponent<Input_Reader>() != null);
            if (otherPlayerTransform)
                otherPlayerInputReaderScript = otherPlayerTransform.GetComponent<Input_Reader>();

            // Debug
            //Debug.Log("[" + gameObject.name + "] Target: " + targetName);
            //Debug.Log("[" + gameObject.name + "] Result: " + otherPlayerInputReaderScript);
        }
    }

    private void OnEnable()
    {
        foreach (Input_Behaviour inputBehaviour in inputBehaviourList)
            queryInputs += inputBehaviour.onKeyPressed;
    }

    private void OnDisable()
    {
        foreach (Input_Behaviour inputBehaviour in inputBehaviourList)
            queryInputs -= inputBehaviour.onKeyPressed;
    }
    // ----------------------------------------
    // Update is called once per frame
    private void Update()
    {
        if (forceIdling && isGrounded)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleAnim))
                anim.Play(idleAnim);
        }

        if (!player || (player && !player.IsAlive()) || queryInputs == null || GameManager.instance.GetGameState() != GameState.START)
            return;

        queryInputs(anim, ref inputsPressedOnFrame, ref motionInputsPressedOnFrame);
        // -----------------------------
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
        // -----------------------------
        // No Inputs Pressed -> Idle or Falling down, depends on isGrounded
        if (motionInputsPressedOnFrame.Count == 0)
        {
            if (isGrounded)
            {
                // Only Do Idle Anim if not attacking, got hit or is blocking.
                if (Mathf.Abs(rbody.velocity.y) <= 0.01f && !isAttacking && !isHit && !isBlocking)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleAnim))
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

    private void FixedUpdate()
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
            UpdatePlayerState(grounded);
    }

    // Nested IFs ftw.
    private void UpdatePlayerState(bool newGroundedState)
    {
        if (isGrounded != newGroundedState)
            isGrounded = newGroundedState;

        // Debug
        //Debug.Log("Updating is Grounded State: " + isGrounded);


        // If we are charging or got hit, enable the following:
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
        if (isHit || isCharging)
        {
            if (isHit) {
                DisableChargeInput();
            }

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
                    if (isBlocking)
                    {
                        DisableGround();
                        DisableAerial();
                    }
                    else
                    {
                        EnableGround();

                        EnableAerialInputAnims();
                        EnableAerialInputs();

                        EnableChargeInput();
                    }
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
                if (isGrounded)
                    DisableGround();
                else
                    DisableGroundAtkInputs();

                DisableAerial();
                DisableChargeInput();
            }
        }
    }
    // ----------------------------------------
    public void StartAttack(string attackName)
    {
        isAttacking = true;

        UpdatePlayerState(isGrounded);
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

        UpdatePlayerState(isGrounded);
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

    public void StopAttack()
    {
        isAttacking = false;

        UpdatePlayerState(isGrounded);
        foreach (Input_Behaviour atkInput in groundAtkInputBehaviourList)
        {
            if (atkInput.isPressed)
                atkInput.isPressed = false;
        }

        Charge_Behaviour chargeBehaviour = GetComponentInChildren<Charge_Behaviour>();
        if (chargeBehaviour.isPressed)
            chargeBehaviour.isPressed = false;
    }
    // ----------------------------------------
    public void StartCharging()
    {
        isCharging = true;
        UpdatePlayerState(isGrounded);
    }

    public void StopCharging()
    {
        isCharging = false;
        UpdatePlayerState(isGrounded);
    }
    // ----------------------------------------
    public void OnHitByAttack()
    {
        if (!isHit)
        {
            isHit = true;
            if (isAttacking)
                StopAttack();
            else
                UpdatePlayerState(isGrounded);
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(hurtAnim))
            anim.Play(hurtAnim);
    }

    public void OnRecoverFromAttack()
    {
        if (isHit)
        {
            isHit = false;
            UpdatePlayerState(isGrounded);
        }
    }
    // ----------------------------------------
    public bool IsMoving()
    {
        return !isAttacking && !isCharging && Input.GetAxis("Horizontal_P" + player.assignedPlayerNum) != 0;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void BeginBlock()
    {
        if (!isBlocking)
        {
            transform.eulerAngles = new Vector3(0, otherPlayerInputReaderScript.transform.position.x < transform.position.x ? 180 : 0, 0);
            isBlocking = true;
            UpdatePlayerState(isGrounded);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(blockAnim))
                anim.Play(blockAnim);
        }
    }

    public void EndBlock()
    {
        if (isBlocking)
        {
            isBlocking = false;
            UpdatePlayerState(isGrounded);
        }
    }
    // ----------------------------------------
    public void ForceToIdleState()
    {
        forceIdling = true;
    }
    // ----------------------------------------
}
