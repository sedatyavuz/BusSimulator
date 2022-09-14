using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwerveHorizontal : MonoBehaviour
{
    #region private
    [HideInInspector] public bool _changeLine = true;
    [Header("LineElements")]
    [SerializeField] private float _lineChangeValueMax;
    [SerializeField] private float _lineChangeValue;
    [SerializeField] private float _lineChangeTime;
    private SplineFollower _follower;
    private Touch _touch;
    private float _touchBeganPositionX;
    private float _screenWidthCalculate;
    #endregion

    private void Start()
    {
        DOTween.Init();
        _follower = GetComponent<SplineFollower>();
        _screenWidthCalculate = Screen.width / 8;
    }

    private void Update()
    {
        if (Input.touchCount == 1 && _changeLine)
        {
            _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                _touchBeganPositionX = _touch.position.x;

            }
            else if ((_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
                && Mathf.Abs(_touchBeganPositionX - _touch.position.x) >= _screenWidthCalculate)
            {
                if (_touch.position.x - _touchBeganPositionX > 0)
                {
                    ChangeLine(+_lineChangeValue);
                }
                else
                {
                    ChangeLine(-_lineChangeValue);
                }
            }
        }
    }

    void ChangeLine(float changeValue)
    {
        _changeLine = false;
        float nextXPosition;
        float currentOffset = 0;
        nextXPosition = Mathf.Clamp(_follower.motion.offset.x + changeValue, 0, _lineChangeValueMax);
        DOTween.To(x => currentOffset = x, _follower.motion.offset.x, nextXPosition, _lineChangeTime)
            .OnUpdate(() =>
            {
                _follower.motion.offset = new Vector2(currentOffset, _follower.motion.offset.y);
            }).OnComplete(() =>
            {
                _changeLine = true;
            });
    }
}
