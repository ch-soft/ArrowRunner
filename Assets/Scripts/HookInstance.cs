﻿using System.Collections;
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

[RequireComponent(typeof(LineRenderer))]
public class HookInstance : MonoBehaviour
{
    [SerializeField] private Transform m_characterTransform;
    [SerializeField] Transform m_hookBase;
    [Space]
    [SerializeField] CoolLettering m_coolLettering;

    public Rigidbody m_characterRigidbody;

    [HideInInspector] public HookState m_hookState;
    [HideInInspector] public Vector3 m_targetPosition;

    private float m_hookMovementSpeed = 35f;

    private Vector3 m_hookLocalStartPosition;
    private Vector3 m_defaultHookScale;
    private Vector3 m_positinonOffset = new Vector3(0f, 0f, 0.01f);

    private LineRenderer m_lineRenderer;

    private Transform m_parent;


    private const int m_grabbingObjectLayer = 8;

    private bool m_isHookInAir;

    private void Awake()
    {
        m_hookLocalStartPosition = transform.localPosition;
        m_parent = gameObject.transform.parent;

        m_defaultHookScale = transform.localScale;

        m_lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        m_hookState = HookState.Based;
    }

    private void FixedUpdate()
    {
        if (m_isHookInAir)
        {
            switch (m_hookState)
            {
                case HookState.FliesToTarget:
                    {
                        ShootGrabHookToTarget();
                        break;
                    }
                case HookState.FliesToBase:
                    {
                        ReturnHookToBase();
                        break;
                    }
            }

            m_lineRenderer.SetPosition(0, m_parent.position);
            m_lineRenderer.SetPosition(1, transform.position);
        }
    }

    private void ShootGrabHookToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, Time.fixedDeltaTime * m_hookMovementSpeed);
    }

    private void ReturnHookToBase()
    {
        transform.position = Vector3.Lerp(transform.position, m_hookBase.position, Time.fixedDeltaTime * m_hookMovementSpeed * 0.3f);
        if (Vector3.Distance(transform.position, m_hookBase.position) < 1f)
        {
            m_hookState = HookState.Based;

            ChangeLocalHookState(false);

            ResetDefaultHookParapemers();
            print("Hook was returned");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isHookInAir)
        {
            switch (other.gameObject.layer)
            {
                case 0:
                case 11:
                case 12:
                case 13:
                case 16:
                    {

                        StartCoroutine(FixateHitAndReturnHome(0.0f));
                        break;
                    }
                case 15:
                    {
                        if (other.gameObject.GetComponent<GrabbingBaseObject>())
                        {
                            if (!other.gameObject.GetComponent<GrabbingBaseObject>().m_objectWasAttracted)
                            {
                                other.gameObject.GetComponent<GrabbingBaseObject>().PrepareGrabbingObject(m_parent.position, transform);
                                other.gameObject.GetComponent<IOnHookGrab>().OnHookGrab();
                            }
                            else
                            {
                                StartCoroutine(FixateHitAndReturnHome(0.0f));
                            }
                        }

                        StartCoroutine(FixateHitAndReturnHome(1.0f));
                        StartCoroutine(m_coolLettering.ShowCoolWord(1.0f));

                        break;
                    }
                case m_grabbingObjectLayer:
                    {
                        //need add more logic here

                        if (other.gameObject.GetComponent<GrabbingBaseObject>())
                        {
                            if (!other.gameObject.GetComponent<GrabbingBaseObject>().m_objectWasAttracted)
                            {
                                other.gameObject.GetComponent<GrabbingBaseObject>().PrepareGrabbingObject(m_parent.position, transform);
                                other.gameObject.GetComponent<IOnHookGrab>().OnHookGrab();
                            }
                            else
                            {
                                StartCoroutine(FixateHitAndReturnHome(0.0f));

                            }
                        }
                        StartCoroutine(FixateHitAndReturnHome(0.0f));
                        StartCoroutine(m_coolLettering.ShowCoolWord(0.0f));

                        break;
                    }
            }
        }

    }

    private IEnumerator FixateHitAndReturnHome(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_hookState = HookState.FliesToBase;

    }

    public void ChangeLocalHookState(bool state)
    {
        m_isHookInAir = state;
        m_lineRenderer.enabled = state;
    }

    public void ResetDefaultHookParapemers()
    {
        transform.parent = m_parent;
        transform.position = m_hookBase.position/* + m_positinonOffset*/;
        transform.localScale = m_defaultHookScale;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
