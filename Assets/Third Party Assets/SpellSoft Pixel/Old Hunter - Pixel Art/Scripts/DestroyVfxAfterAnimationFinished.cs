using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVfxAfterAnimationFinished : MonoBehaviour
{
    private void DestroyVfxObject()
    {
        Transform parentTrans = GetComponentInParent<Transform>();
        Destroy(parentTrans.gameObject);
    }
}
