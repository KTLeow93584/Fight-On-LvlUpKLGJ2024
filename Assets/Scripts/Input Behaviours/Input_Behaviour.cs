using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Behaviour : MonoBehaviour
{
    // ----------------------------------------
    [SerializeField]
    protected Player_Character player = null;

    [SerializeField]
    protected Rigidbody rbody;

    public string targetAnimName = "idle";

    [SerializeField]
    public bool isBehaviourEnabled = false;

    [SerializeField]
    public bool isAnimEnabled = false;

    public bool isPressed = false;
    // ----------------------------------------
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!rbody)
            rbody = GetComponent<Rigidbody>();
    }
    // ----------------------------------------
    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isBehaviourEnabled)
            return;
    }
    // ----------------------------------------
    public virtual void onKeyPressed(Animator anim, ref List<int> inputHistory, ref List<int> motionHistory)
    {
        anim.Play(targetAnimName);
    }
    // ----------------------------------------
}
