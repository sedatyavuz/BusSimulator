using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private enum Tutorial { HorizontalSwerve = 0, JoystickSwerve = 1, Drag = 2 }
    [SerializeField] private Tutorial tutorial;


    public int GetTutorialIndex()
    {
        return (int)tutorial;
    }
}
