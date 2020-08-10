using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum HookState
{
    None = 0,
    Based,
    FliesToTarget,
    HitTarget,
    FliesToBase
}

public class HookInstance : MonoBehaviour
{
    [HideInInspector] public HookState m_hookState;
    [HideInInspector] public Vector3 m_targetPosition;

    private float m_hookMovementSpeed = 20f;

    private Vector3 m_hookLocalStartPosition;

    private Transform m_parent;

    private const int m_grabbingObjectLayer = 8;

    private void Awake()
    {
        m_hookState = HookState.Based;
        m_hookLocalStartPosition = transform.localPosition;
        m_parent = gameObject.transform.parent;
    }

    private void Update()
    {
        switch (m_hookState)
        {
            case HookState.FliesToBase:
                {
                    ShootGrabHookToTarget();
                    break;
                }
            case HookState.FliesToTarget:
                {
                    ReturnHookToBase();
                    break;
                }
        }
    }

    private void ShootGrabHookToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, Time.deltaTime * m_hookMovementSpeed);
    }

    private void ReturnHookToBase()
    {
        transform.localPosition = Vector3.MoveTowards(transform.position, m_hookLocalStartPosition, Time.deltaTime * m_hookMovementSpeed * 3f);
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case m_grabbingObjectLayer:
                {

                    break;
                }
        }
    }

}
