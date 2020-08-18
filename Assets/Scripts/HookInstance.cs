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

[RequireComponent(typeof(LineRenderer))]
public class HookInstance : MonoBehaviour
{
    [SerializeField] private Transform m_characterTransform;
    [SerializeField] public Rigidbody m_characterRigidbody;

    [HideInInspector] public HookState m_hookState;
    [HideInInspector] public Vector3 m_targetPosition;

    private float m_hookMovementSpeed = 35f;

    private Vector3 m_hookLocalStartPosition;
    private Vector3 m_defaultHookScale;

    private LineRenderer m_lineRenderer;

    private Transform m_parent;


    private const int m_grabbingObjectLayer = 8;

    private bool m_isHookInAir;

    private void Awake()
    {
        m_hookState = HookState.Based;
        m_hookLocalStartPosition = transform.localPosition;
        m_parent = gameObject.transform.parent;

        m_defaultHookScale = transform.localScale;

        m_lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
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
        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, Time.deltaTime * m_hookMovementSpeed);
    }

    private void ReturnHookToBase()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_hookLocalStartPosition, Time.deltaTime * m_hookMovementSpeed * 4f);
        if (Vector3.Distance(transform.localPosition, m_hookLocalStartPosition) < 0.1f)
        {
            transform.localPosition = m_hookLocalStartPosition;
            m_hookState = HookState.Based;
            ChangeLocalHookState(false);

            ResetDefaultHookParapemers();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 0:
            case 11:
            case 13:
                {
                    StartCoroutine(FixateHitAndReturnHome(0.0f));
                    break;
                }
            case 15:
                {
                    if (other.gameObject.GetComponent<GrabbingBaseObject>())
                    {
                        other.gameObject.GetComponent<GrabbingBaseObject>().PrepareGrabbingObject(m_parent.position, transform);
                        other.gameObject.GetComponent<IOnHookGrab>().OnHookGrab();
                    }

                    StartCoroutine(FixateHitAndReturnHome(1.2f));
                    break;
                }
            case m_grabbingObjectLayer:
                {
                    //need add more logic here

                    if (other.gameObject.GetComponent<GrabbingBaseObject>())
                    {
                        other.gameObject.GetComponent<GrabbingBaseObject>().PrepareGrabbingObject(m_parent.position, transform);
                        other.gameObject.GetComponent<IOnHookGrab>().OnHookGrab();
                    }

                    StartCoroutine(FixateHitAndReturnHome(0.0f));
                    break;
                }
        }
    }

    private IEnumerator FixateHitAndReturnHome(float delay)
    {

        yield return new WaitForSeconds(delay);
        transform.parent = m_parent;
        m_hookState = HookState.FliesToBase;
    }

    public void ChangeLocalHookState(bool state)
    {
        m_isHookInAir = state;
        m_lineRenderer.enabled = state;
    }

    private void ResetDefaultHookParapemers()
    {
        transform.localScale = m_defaultHookScale;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
