using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float lerpSpeed = 0.125f;
    [SerializeField] private bool CanLookAt = false;
    Vector3 targetPosition;
    [SerializeField] private Transform target;
    public bool isCameraFollow;

    private void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void LateUpdate()
    {
        if (isCameraFollow)
        {
            if (target != null)
            {
                targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);

                transform.position = targetPosition;
                transform.rotation = target.rotation;
                //transform.eulerAngles = target.eulerAngles;
            }
            if (CanLookAt)
            {
                transform.LookAt(target.localPosition);
            }
        }

    }
}
