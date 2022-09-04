using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickBase : MonoBehaviour
{
    [SerializeField] protected float MoveSpeed = 1f;
    [SerializeField] protected bool HasRigidbody = false;

    private void Start()
    {
        if (HasRigidbody)
        {
            JoystickRb joystickRb = gameObject.AddComponent<JoystickRb>();
            joystickRb.SetJoystick(MoveSpeed);
        }
        else
        {
            JoystickTransform joystickTransform = gameObject.AddComponent<JoystickTransform>();
            joystickTransform.SetJoystick(MoveSpeed);
        }
    }
}
