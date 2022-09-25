using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsSc : MonoBehaviour
{
    [SerializeField] private float lineChangeValue;
    private SplineFollower follower;
    public enum Line { rightLine, rightOutLine, leftLine ,leftOutLine}
    [SerializeField] public Line line;

    private void Start()
    {
        follower = GetComponent<SplineFollower>();
        if(line == Line.rightLine)
        {
            follower.motion.offset = new Vector3(lineChangeValue, 0, 0);
        }
        if (line == Line.rightOutLine)
        {
            follower.motion.offset = new Vector3(lineChangeValue*3, 0, 0);
        }
        if (line == Line.leftLine)
        {
            follower.motion.offset = new Vector3(lineChangeValue*-1, 0, 0);
        }
        if (line == Line.leftOutLine)
        {
            follower.motion.offset = new Vector3(lineChangeValue*-3, 0, 0);
        }
    }
}
