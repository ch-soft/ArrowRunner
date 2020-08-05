﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class CameraController : MonoBehaviour
{
    [BoxGroup("Parameters"), SerializeField] private GameObject _target;
    [BoxGroup("Parameters"), SerializeField] private Vector3 _offset;
    [BoxGroup("Parameters"), SerializeField] private float _durability; //from 0 to 1

    private float m_interpVelocity;
    private float m_interpVelocityForce = 20f;

    private Vector3 m_targetPos;

    void Start()
    {
        m_targetPos = transform.position;
    }

    void FixedUpdate()
    {
        FollowForPlayer();
    }

    private void FollowForPlayer()
    {
        if (_target)
        {
            Vector3 nativePosition = transform.position;

            Vector3 targetDirection = (_target.transform.position - nativePosition);

            m_interpVelocity = targetDirection.magnitude * m_interpVelocityForce;

            m_targetPos = transform.position + (targetDirection.normalized * m_interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, m_targetPos + _offset, _durability);
        }
    }
}
