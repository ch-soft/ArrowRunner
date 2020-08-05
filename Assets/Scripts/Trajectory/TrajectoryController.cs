using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    [Header("Line renderer veriables")]
    public LineRenderer _line;
    [Range(2, 30)]
    public int _resolution;

    [Header("Formula variables")]
    public Vector2 _velocity;
    public float _yLimit; //for later
    private float _forceValue;

    [Header("Linecast variables")]
    [Range(2, 30)]
    public int linecastResolution;
    public LayerMask canHit;

    private float _maxTrajectoryDistance = 12f;


    private void Start()
    {
        _forceValue = Mathf.Abs(Physics2D.gravity.y);
    }

    private void Update()
    {
        CalculateArcAngle();
        StartCoroutine(RenderArc());
    }

    private IEnumerator RenderArc()
    {
        _line.positionCount = _resolution + 1;
        _line.SetPositions(CalculateLineArray());
        yield return null;
    }

    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[_resolution + 1];

        var lowestTimeValue = MaxTimeX() / _resolution;

        for (int i = 0; i < lineArray.Length; i++)
        {
            var t = lowestTimeValue * i;
            lineArray[i] = CalculateLinePoint(t);
        }

        return lineArray;
    }

    private Vector2 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / linecastResolution;

        for (int i = 0; i < linecastResolution + 1; i++)
        {
            var t = lowestTimeValue * i;
            var tt = lowestTimeValue * (i + 1);

            var hit = Physics2D.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), canHit);

            if (hit)
                return hit.point;
        }

        return CalculateLinePoint(MaxTimeY());
    }

    private void CalculateArcAngle()
    {
        if (_velocity.x < _maxTrajectoryDistance)
        {
            if ((_velocity.x >= 0) && (_velocity.x < _maxTrajectoryDistance / 2f))
            {
                _velocity = new Vector2(_velocity.x, _maxTrajectoryDistance - _velocity.x);
            }
            else if ((_velocity.x >= _maxTrajectoryDistance / 2f) && (_velocity.x < Mathf.Infinity))
            {
                float multiplier = (_velocity.x - _maxTrajectoryDistance / 2f);
                _velocity = new Vector2(_velocity.x, (_maxTrajectoryDistance - _velocity.x) + multiplier);
            }
        }
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = _velocity.x * t;
        float y = (_velocity.y * t) - (_forceValue * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y);
    }

    private float MaxTimeY()
    {
        var v = _velocity.y;
        var vv = v * v;

        var t = (v + Mathf.Sqrt(vv + 2 * _forceValue * (transform.position.y - _yLimit))) / _forceValue;
        return t;
    }

    private float MaxTimeX()
    {
        var x = _velocity.x;
        if (x == 0)
        {
            _velocity.x = 000.1f;
            x = _velocity.x;
        }

        var t = (HitPosition().x - transform.position.x) / x;
        return t;
    }
}