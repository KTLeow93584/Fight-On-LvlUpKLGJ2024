using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    [SerializeField]
    private Transform anchorP1 = null;

    [SerializeField]
    private Transform anchorP2 = null;

    [SerializeField]
    private float sensitivity = 1.2f;

    [SerializeField]
    private float maxXDistance = 0.0f;

    [SerializeField]
    private float minXDistance = 0.0f;

    private float originalZPosition = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        if (!anchorP1)
            anchorP1 = GameObject.Find("P1").transform;

        if (!anchorP2)
            anchorP2 = GameObject.Find("P2").transform;

        originalZPosition = transform.position.z;
    }

    // Update is called once per frame
    private void Update()
    {
        float distanceX = GetDistanceXBetweenAnchors();
        float zOffset = GetZPositionOffset(distanceX);

        if (distanceX >= GetDistanceXBetweenAnchors())

        transform.position = new Vector3((anchorP1.position.x + anchorP2.position.x) * 0.5f, transform.position.y, originalZPosition - zOffset);
    }

    private float GetDistanceXBetweenAnchors()
    {
        return Mathf.Abs(anchorP1.position.x - anchorP2.position.x);
    }

    private float GetZPositionOffset(float distance)
    {
        float maxGap = maxXDistance - minXDistance;
        return Mathf.Clamp01(((distance - minXDistance) / maxGap)) * (maxGap * sensitivity);
    }
}
