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

    [SerializeField]
    private Collider hitbox = null;

    [SerializeField]
    private SpriteRenderer spriteRenderer = null;
    // -------------------------------
    [SerializeField]
    private Health_Component health = null;

    [SerializeField]
    private Energy_Component energy = null;

    [SerializeField]
    private float energyLeechMultiplier = 1.0f;
    // -------------------------------
    [SerializeField]
    private Input_Reader readerScript = null;

    private Coroutine blockCoroutine = null;
    // -------------------------------
    [SerializeField]
    private float primaryAttackDamage = 0.0f;

    [SerializeField]
    private float secondaryAttackDamage = 0.0f;

    [SerializeField]
    private float comboDamage = 0.0f;

    [SerializeField]
    private float blockDuration = 0.6f;
    // -------------------------------
    [SerializeField]
    private string deathAnim = "Death";

    [SerializeField]
    private string victoryAnim = "Heal";
    // -------------------------------
    [SerializeField]
    private float attackDamage = 0.0f;

    [SerializeField]
    private bool isAlive = false;
    // -------------------------------
    private void OnEnable()
    {
        OnInit(assignedPlayerNum);

        if (hitbox && !hitbox.gameObject.activeSelf)
            hitbox.gameObject.SetActive(true);

        isAlive = true;

        if (health)
            health.isRegenDisabled = true;

        if (spriteRenderer)
            spriteRenderer.enabled = true;
    }

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
                {
                    Transform shadowHealthBar = child.Find("Shadow Health Bar");
                    health.SetUIComponent(shadowHealthBar, child);
                    health.isRegenDisabled = true;
                }
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
    public void OnInit(int playerNumber)
    {
        transform.eulerAngles = new Vector3(0.0f, playerNumber % 2 == 0 ? 180.0f : 0.0f, 0.0f);
    }
    // -------------------------------
    public void OnHitByAttack(Transform attackSource, float damage)
    {
        // ----------------------------
        int direction = attackSource.position.x < transform.position.x ? -1 : 1;

        // Turn to face our attacker.
        transform.eulerAngles = new Vector3(0.0f, direction == -1 ? 180.0f : 0.0f, 0.0f);
        // ----------------------------
        // If Attack is coming from the Left and I am holding the Right Key.
        // Same if the Attack is coming from the Right and I am holding the Left Key.
        // Block The Attack.
        bool canBlock =
            (direction == -1 && Input.GetAxis("Horizontal_P" + assignedPlayerNum) > 0) ||
            (direction == 1 && Input.GetAxis("Horizontal_P" + assignedPlayerNum) < 0);

        if (canBlock)
        {
            float blockCost = -damage * 0.5f;

            // Debug
            //Debug.Log("[On Receive Hit] To block this attack, I need " + blockCost + " energy.");
            //Debug.Log("[On Receive Hit] I currently have " + energy.GetCurrentValue() + " energy.");

            if (energy.GetCurrentValue() >= Mathf.Abs(blockCost))
            {
                if (readerScript.IsGrounded())
                    readerScript.BeginBlock();

                if (blockCoroutine != null)
                    StopCoroutine(blockCoroutine);
                blockCoroutine = StartCoroutine("StartTimeToEndBlock");
                // ----------------------------
                // Debug
                //Debug.Log("I was hit by " + attackSource.gameObject.name + " but it was successfully blocked!");
                // ----------------------------
                energy.AddValue(blockCost);
                return;
            }
            else
            {
                // Debug
                //Debug.Log("[On Receive Hit] Failed to Block Attack due to lack of energy!");
            }
        }
        // Otherwise, Take Damage

        // Debug
        //Debug.Log("[On Receive Hit] I was hit by " + attackSource.gameObject.name + " for " + damage + " damage.");
        // ----------------------------
        // Update Reader Script
        // - Stop all existing input processes (E.g. isAttacking, isCharging)
        // - Prevent Idle Animation, use Hit Animation instead.
        readerScript.OnHitByAttack();
        // ----------------------------
        // Take Damage
        health.AddValue(-damage);
        // ----------------------------
        Player_Character otherCharacterScript = attackSource.GetComponent<Player_Character>();

        // Debug
        //Debug.Log("Other Character: " + otherCharacterScript);

        // Gain Energy for hitting target.
        if (otherCharacterScript)
            otherCharacterScript.energy.AddValue(damage * energyLeechMultiplier);
        // ----------------------------
        if (health.GetCurrentValue() <= 0)
        {
            isAlive = false;
            StartCoroutine("OnDeath");
        }
        // ----------------------------
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void OnBeginPrimaryAttack()
    {
        attackDamage = primaryAttackDamage;
    }

    public void OnBeginSecondaryAttack()
    {
        attackDamage = secondaryAttackDamage;
    }

    public void OnBeginCombo()
    {
        attackDamage = comboDamage;
    }
    // -------------------------------
    public bool IsAlive()
    {
        return isAlive;
    }
    // -------------------------------
    IEnumerator StartTimeToEndBlock()
    {
        yield return new WaitForSeconds(blockDuration);
        readerScript.EndBlock();
    }
    // -------------------------------
    protected IEnumerator OnDeath()
    {
        // Have Animation = Play Animation
        if (deathAnim != "")
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(deathAnim))
                anim.Play(deathAnim);
        }
        // Otherwise = Just Disappear
        else
        {
            if (spriteRenderer)
                spriteRenderer.enabled = false;
        }

        health.isRegenDisabled = false;

        GameManager.instance.OnGameEnded();
        yield return null;
    }
    // -------------------------------
    public float GetHealth()
    {
        return health.GetCurrentValue();
    }

    public void OnForceDefeat()
    {
        StartCoroutine(OnDeath());
    }

    public void OnVictory()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(victoryAnim))
            anim.Play(victoryAnim);
    }

    public void OnMatchTied()
    {
        readerScript.ForceToIdleState();
    }
    // -------------------------------
}
