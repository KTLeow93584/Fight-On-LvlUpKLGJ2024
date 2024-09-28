using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Player_Character : MonoBehaviour
{
    // -------------------------------
    public int assignedPlayerNum = 1;
    // -------------------------------
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Rigidbody rbody;
    // -------------------------------
    [SerializeField]
    private Health_Component health = null;

    [SerializeField]
    private Energy_Component energy = null;
    // -------------------------------
    private string idleAnim = "Idle";
    //private string crouchAnim = "Idle";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string rollAnim = "Roll";
    private string hurtAnim = "Hurt";
    private string blockAnim = "Block";
    private string deathAnim = "Death";
    private string attack1Anim = "Attack1";
    private string attack2Anim = "Attack2";
    private string attack3Anim = "Attack3";
    private string healAnim = "Heal";
    // -------------------------------
    [SerializeField]
    private KeyCode attackKey = KeyCode.P;
    // -------------------------------
    // Rolling variables
    private float rollForce = 25f;
    // -------------------------------
    // Motion State variables
    private bool isGrounded;
    private bool isHoldingBlock;
    private bool isAttacking;
    private bool isDead;
    [SerializeField]
    private bool canContinueAttackCombo;
    [SerializeField]
    private int currentAttackAnim;
    // -------------------------------
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        Transform playerTransform = GameObject.Find("P" + assignedPlayerNum.ToString()).transform;

        if (playerTransform)
        {
            foreach (Transform child in playerTransform)
            {
                if (child.gameObject.name == "Health Bar")
                    health.SetUIComponent(child);
                if (child.gameObject.name == "Energy Bar")
                {
                    energy.SetUIComponent(child);
                    energy.SetIndicatorText(child.Find("Counter"));
                }
            }
        }

        ResetState();
    }

    void ResetState()
    {
        isHoldingBlock = false;
        isAttacking = false;
        isDead = false;
        canContinueAttackCombo = false;
        currentAttackAnim = 0;
    }
    // -------------------------------
    // Update is called once per frame
    void Update()
    {
        //GetMoveInput();
        //GetJumpInput();
        //GetRollInput();
        //GetGetHitInput();
        //GetHoldBlockInput();
        //GetDeadInput();
        //GetAttackInput();
        //GetHealInput();
        //FlipSprite();
    }
    // -------------------------------
    #region INPUTS
    //private void GetMoveInput()
    //{
    //    if (canReceiveInput)
    //    {
    //        float moveForce = (Input.GetAxis("Horizontal_P" + assignedPlayerNum));

    //        if (moveForce < 0)
    //        {
    //            isRunningLeft = true;
    //            isRunningRight = false;
    //        }
    //        else if (moveForce > 0)
    //        {
    //            isRunningRight = true;
    //            isRunningLeft = false;
    //        }

    //        if (Input.GetAxis("Horizontal_P" + assignedPlayerNum) == 0)
    //            StopMovement();
    //    }
    //}
    //private void GetJumpInput()
    //{
    //    if (canReceiveInput)
    //    {
    //        if (Input.GetAxis("Jump_P" + assignedPlayerNum) > 0 && isGrounded)
    //            Jump();
    //    }
    //}
    //private void GetRollInput()
    //{
    //    if (canReceiveInput)
    //    {
    //        if (Input.GetAxis("Roll_P" + assignedPlayerNum) > 0)
    //            Roll();
    //    }
    //}
    // private void GetGetHitInput()
    // {
    //     if (canReceiveInput)
    //     {
    //         if (Input.GetKeyDown(KeyCode.T))
    //             GetHit();
    //     }
    // }
    
    //private void GetHoldBlockInput()
    //{
    //    if (canReceiveInput)
    //    {
    //        if (Input.GetAxis("Block_P" + assignedPlayerNum) > 0)
    //            HoldBlock();
    //    }
    //    else
    //    {
    //        if (isHoldingBlock)
    //            EndBlocking();
    //    }
    //}

    // private void GetDeadInput()
    // {
    //     if (canReceiveInput)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Y))
    //         {
    //             Die();
    //         }
    //     }
    //     else if (!canReceiveInput && isDead)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Y))
    //         {
    //             ResetDeath();
    //         }
    //     }
    // }

    //private void GetAttackInput()
    //{
    //    if (canReceiveInput)
    //    {
    //        if (Input.GetKeyDown(attackKey))
    //            Attack();
    //    }
    //    else
    //    {
    //        if (Input.GetKeyDown(attackKey) && canContinueAttackCombo)
    //            Attack();
    //    }
    //}

    // private void GetHealInput()
    // {
    //     if (canReceiveInput)
    //     {
    //         if (Input.GetKeyDown(KeyCode.O))
    //         {
    //             Heal();
    //         }
    //     }
    // }

    //private void ReEnableInput()
    //{
    //    canReceiveInput = true;
    //}
    #endregion
    // -------------------------------
    #region ROLLING LOGIC
    //private void Roll()
    //{
    //    canReceiveInput = false;
    //    StopMovement();
    //    rbody.velocity = new Vector2(rollForce * GetPlayerDirection(), rbody.velocity.y);
    //    anim.Play(rollAnim);
    //}
    #endregion
    // -------------------------------
    #region GETHIT LOGIC
    //private void GetHit()
    //{
    //    canReceiveInput = false;
    //    StopMovement();
    //    anim.Play(hurtAnim);
    //}
    #endregion
    // -------------------------------
    #region HOLDBLOCK LOGIC
    //private void HoldBlock()
    //{
    //    isHoldingBlock = true;
    //    canReceiveInput = false;
    //    StopMovement();
    //    anim.Play(blockAnim);
    //}
    //private void EndBlocking()
    //{
    //    ReEnableInput();
    //    isHoldingBlock = false;
    //}
    #endregion
    // -------------------------------
    #region DEATH LOGIC
    //private void Die()
    //{
    //    isDead = true;
    //    canReceiveInput = false;
    //    StopMovement();
    //    anim.Play(deathAnim);
    //}
    //private void ResetDeath()
    //{
    //    isDead = false;
    //    ReEnableInput();
    //}
    #endregion
    // -------------------------------
    #region ATTACK LOGIC
    //private void Attack()
    //{
    //    if (!canContinueAttackCombo)
    //    {
    //        canReceiveInput = false;
    //        isAttacking = true;
    //        //StopMovement();
    //        currentAttackAnim = 0;
    //        DisableCanContinueAttackCombo();
    //        anim.Play(attack1Anim);
    //        currentAttackAnim++;
    //    }
    //    else
    //    {
    //        if (currentAttackAnim == 0)
    //        {
    //            DisableCanContinueAttackCombo();
    //            anim.Play(attack1Anim);
    //            currentAttackAnim++;
    //        }
    //        else if (currentAttackAnim == 1)
    //        {
    //            DisableCanContinueAttackCombo();
    //            anim.Play(attack2Anim);
    //            currentAttackAnim++;
    //        }
    //        else if (currentAttackAnim == 2)
    //        {
    //            DisableCanContinueAttackCombo();
    //            anim.Play(attack3Anim);
    //            currentAttackAnim = 0;
    //        }
    //    }
    //}
    //private void EnableCanContinueAttackCombo()
    //{
    //    canContinueAttackCombo = true;
    //}
    //private void DisableCanContinueAttackCombo()
    //{
    //    canContinueAttackCombo = false;
    //}
    #endregion
    // -------------------------------
    #region HEAL LOGIC
    //private void Heal()
    //{
    //    //canReceiveInput = false;
    //    //StopMovement();
    //    anim.Play(healAnim);
    //}
    #endregion
    // -------------------------------
    #region FACE DIRECTION
    //private float GetPlayerDirection()
    //{
    //    return transform.localScale.x;
    //}

    //private void FlipSprite()
    //{
    //    if (rbody.velocity.x < 0)
    //    {
    //        transform.eulerAngles = new Vector3(0, 180, 0);
    //    }
    //    else if (rbody.velocity.x > 0)
    //    {
    //        transform.eulerAngles = new Vector3(0, 0, 0);
    //    }
    //}
    #endregion
    // -------------------------------
}
