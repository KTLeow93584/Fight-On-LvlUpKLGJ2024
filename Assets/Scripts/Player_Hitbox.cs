using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hitbox : MonoBehaviour
{
    // -------------------------------
    [SerializeField]
    Player_Character playerScript = null;

    private Collider localCollider = null;
    // -------------------------------
    // Start is called before the first frame update
    void Start()
    {
        if (!localCollider)
            localCollider = GetComponent<Collider>();
    }
    // -------------------------------
    private void OnTriggerEnter(Collider foreignCollider)
    {
        if (!playerScript)
            return;

        GameObject playerParentGO = foreignCollider.transform.parent.gameObject;
        
        // Got Hit by Opposing Player's Attack Hitbox
        if (playerParentGO.gameObject.tag != playerScript.gameObject.tag)
            playerScript.OnHitByAttack(playerParentGO.transform, playerParentGO.GetComponent<Player_Character>().GetAttackDamage());
    }
    // -------------------------------
}
