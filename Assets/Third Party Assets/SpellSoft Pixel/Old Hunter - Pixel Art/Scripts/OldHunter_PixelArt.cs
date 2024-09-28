/*
 * Author: Dimitrios Gkaltsidis
 * Date: 27 Sept 2023
 * Disclaimer: This code is not fully optimized. For production-level 2D character functionality, consider crafting your own.
 * Version: 1.0.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldHunter_PixelArt : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator animator;

    // VFX
    [SerializeField] GameObject vfxObject;

    // Animation names for animator
    private string idleAnim = "Idle";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string rollAnim = "Roll";
    private string hurtAnim = "Hurt";
    private string blockAnim = "Block";
    private string blockImpactAnim = "BlockImpact";
    private string deathAnim = "Death";
    private string deathNoBloodAnim = "DeathNoBlood";
    private string attack1Anim = "Attack1";
    private string attack2Anim = "Attack2";
    private string attack3Anim = "Attack3";
    private string specialAttack = "SpecialAttack";

    // Movement variables
    private float moveSpeed = 8f;
    private bool isRunningLeft;
    private bool isRunningRight;

    // Jumping variables
    private float jumpForce = 8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckRadius = 0.2f;

    // Rolling variables
    private float rollForce = 25f;

    // Other variables
    private bool isGrounded;
    private bool isHoldingBlock;
    private bool isDead;
    private bool canContinueAttackCombo;
    private int currentAttackAnim;

    // Can receive input
    private bool canReceiveInput;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        canReceiveInput = true;
        isHoldingBlock = false;
        isDead = false;
        canContinueAttackCombo = false;
        currentAttackAnim = 0;
    }

    private void Update()
    {
        GetMoveInput();
        GetJumpInput();
        GetRollInput();
        GetGetHitInput();
        GetHoldBlockInput();
        GetDeadInput();
        GetAttackInput();
        GetSpecialAttackInput();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        Run();
        CheckIfGrounded();
    }

    #region INPUTS
    private void GetMoveInput()
    {
        if (canReceiveInput)
        {
            float moveForce = (Input.GetAxis("Horizontal"));

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

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                StopMovement();
            }
        }
    }
    private void GetJumpInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    Jump();
                }
            }
        }
    }
    private void GetRollInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Roll();
            }
        }
    }
    private void GetGetHitInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                GetHit();
            }
        }
    }
    private void GetHoldBlockInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HoldBlock();
            }
        }
        else
        {
            if (isHoldingBlock && Input.GetKeyDown(KeyCode.E))
            {
                GetHitWhileBlocking();
            }
            else if (isHoldingBlock && Input.GetKeyUp(KeyCode.Q))
            {
                EndBlocking();
            }
        }
    }
    private void GetDeadInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Die();
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                DieNoBlood();
            }
        }
        else if (!canReceiveInput && isDead)
        {
            if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.U))
            {
                ResetDeath();
            }
        }
    }
    private void GetAttackInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Attack();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P) && canContinueAttackCombo)
            {
                Attack();
            }
        }
    }
    private void GetSpecialAttackInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SpecialAttack();
            }
        }
    }

    private void ReEnableInput()
    {
        canReceiveInput = true;
    }
    #endregion

    #region IDLE & RUN LOGIC
    private void Run()
    {
        if (isRunningLeft)
        {
            rb2D.velocity = new Vector2(-moveSpeed, rb2D.velocity.y);
            if (canReceiveInput)
            {
                animator.Play(runAnim);
            }
        }
        else if (isRunningRight)
        {
            rb2D.velocity = new Vector2(moveSpeed, rb2D.velocity.y);
            if (canReceiveInput)
            {
                animator.Play(runAnim);
            }
        }
        else if (!isRunningLeft && !isRunningRight && canReceiveInput)
        {
            if (canReceiveInput)
            {
                animator.Play(idleAnim);
            }
        }
    }
    private void StopMovement()
    {
        if (isRunningLeft || isRunningRight)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }

        isRunningLeft = false;
        isRunningRight = false;
    }
    #endregion

    #region JUMPING LOGIC
    private void Jump()
    {
        canReceiveInput = false;
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        animator.Play(jumpAnim);
        InvokeRepeating("EndJump", 0.1f, 0.1f);
    }
    private void EndJump()
    {
        if (isGrounded)
        {
            ReEnableInput();
            CancelInvoke();
        }
    }
    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region ROLLING LOGIC
    private void Roll()
    {
        canReceiveInput = false;
        StopMovement();
        rb2D.velocity = new Vector2(rollForce * GetPlayerDirection(), rb2D.velocity.y);
        animator.Play(rollAnim);
    }
    #endregion

    #region GETHIT LOGIC
    private void GetHit()
    {
        canReceiveInput = false;
        StopMovement();
        animator.Play(hurtAnim);
    }
    #endregion

    #region HOLDBLOCK LOGIC
    private void HoldBlock()
    {
        isHoldingBlock = true;
        canReceiveInput = false;
        StopMovement();
        animator.Play(blockAnim);
    }
    private void GetHitWhileBlocking()
    {
        // isGettingHit = true;
        animator.Play(blockImpactAnim);
    }
    private void ContinueBlocking()
    {
        //isGettingHit = false;
        animator.Play(blockAnim);
    }
    private void EndBlocking()
    {
        ReEnableInput();
        isHoldingBlock = false;
    }
    #endregion

    #region DEATH LOGIC
    private void Die()
    {
        isDead = true;
        canReceiveInput = false;
        StopMovement();
        animator.Play(deathAnim);
    }
    private void DieNoBlood()
    {
        isDead = true;
        canReceiveInput = false;
        StopMovement();
        animator.Play(deathNoBloodAnim);
    }
    private void ResetDeath()
    {
        isDead = false;
        ReEnableInput();
    }
    #endregion

    #region ATTACK LOGIC
    private void Attack()
    {
        if (!canContinueAttackCombo)
        {
            canReceiveInput = false;
            StopMovement();
            currentAttackAnim = 0;
            DisableCanContinueAttackCombo();
            animator.Play(attack1Anim);
            currentAttackAnim++;
        }
        else
        {
            if (currentAttackAnim == 0)
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack1Anim);
                currentAttackAnim++;
            }
            else if (currentAttackAnim == 1)
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack2Anim);
                currentAttackAnim++;
            }
            else if (currentAttackAnim == 2)
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack3Anim);
                currentAttackAnim = 0;
            }
        }
    }
    private void EnableCanContinueAttackCombo()
    {
        canContinueAttackCombo = true;
    }
    private void DisableCanContinueAttackCombo()
    {
        canContinueAttackCombo = false;
    }
    #endregion

    #region SPECIAL ATTACK LOGIC
    private void SpecialAttack()
    {
        canReceiveInput = false;
        StopMovement();
        animator.Play(specialAttack);
    }
    #endregion

    #region SPAWN VFX OBJECT
    private void SpawnVFX()
    {
        GameObject myVfx = Instantiate(vfxObject);
        myVfx.transform.position = gameObject.transform.position;
        myVfx.transform.localScale = gameObject.transform.localScale;
    }
    #endregion

    private float GetPlayerDirection()
    {
        return transform.localScale.x;
    }

    private void FlipSprite()
    {
        if (rb2D.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb2D.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
