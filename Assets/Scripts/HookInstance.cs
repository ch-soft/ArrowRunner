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

public enum ReturnableObject
{
    None = 0,
    Barrel,
    Enemy,
    Other
}

[RequireComponent(typeof(LineRenderer))]
public class HookInstance : MonoBehaviour
{
    [SerializeField] private Transform m_hookBase;
    [SerializeField] private Transform m_handMesh;
    [Space]
    [SerializeField] private CoolLettering m_coolLettering;

    [HideInInspector] public HookState m_hookState;
    [HideInInspector] public Vector3 m_targetPosition;
    [HideInInspector] public ReturnableObject m_returnableObject;

    private float m_hookMovementSpeed = 35f;

    private float m_enemyReturnSpeed = 0.12f;
    private float m_barrelReturnSpeed = 0.17f;

    private Vector3 m_defaultHookScale;

    private LineRenderer m_lineRenderer;

    private Transform m_parent;

    private bool m_isHookInAir;

    private void Awake()
    {
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
        if (m_handMesh.localScale.x < 2f)
        {
            m_handMesh.localScale += Vector3.one / 20f;
        }
    }

    private void ReturnHookToBase()
    {
        switch (m_returnableObject)
        {
            case ReturnableObject.Enemy:
            case ReturnableObject.Other:
                {
                    if ((Vector3.Distance(transform.position, m_hookBase.position) >= 2f) && (Vector3.Distance(transform.position, m_hookBase.position) < 100f))
                    {
                        transform.position = Vector3.Lerp(transform.position, m_hookBase.position, Time.fixedDeltaTime * m_hookMovementSpeed * m_enemyReturnSpeed);
                    }
                    else if (Vector3.Distance(transform.position, m_hookBase.position) < 3f)
                    {
                        transform.position = Vector3.Lerp(transform.position, m_hookBase.position, Time.fixedDeltaTime * m_hookMovementSpeed);
                    }

                    if (m_handMesh.localScale.x > 1f)
                    {
                        m_handMesh.localScale -= Vector3.one / 10f;
                    }
                    if (Vector3.Distance(transform.position, m_hookBase.position) < 1f)
                    {
                        m_hookState = HookState.Based;

                        ChangeLocalHookState(false);

                        ResetDefaultHookParapemers();

                        ResetTransform();
                    }
                    break;
                }
            case ReturnableObject.Barrel:
                {
                    if ((Vector3.Distance(transform.position, m_hookBase.position) >= 2f) && (Vector3.Distance(transform.position, m_hookBase.position) < 100f))
                    {
                        transform.position = Vector3.Lerp(transform.position, m_hookBase.position, Time.fixedDeltaTime * m_hookMovementSpeed * m_barrelReturnSpeed);
                    }
                    else if (Vector3.Distance(transform.position, m_hookBase.position) < 3f)
                    {
                        transform.position = Vector3.Lerp(transform.position, m_hookBase.position, Time.fixedDeltaTime * m_hookMovementSpeed);
                    }

                    if (m_handMesh.localScale.x > 1f)
                    {
                        m_handMesh.localScale -= Vector3.one / 10f;
                    }
                    if (Vector3.Distance(transform.position, m_hookBase.position) < 1f)
                    {
                        m_hookState = HookState.Based;

                        ChangeLocalHookState(false);

                        ResetDefaultHookParapemers();

                        ResetTransform();
                    }
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isHookInAir)
        {
            switch (other.gameObject.layer)
            {
                case 0:
                case 10:
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

                        StartCoroutine(FixateHitAndReturnHome(0.5f));
                        StartCoroutine(m_coolLettering.ShowCoolWord(0.5f));
                        break;
                    }
                case 8:
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
                        if (!other.gameObject.GetComponent<GrabbingEnemy>())
                        {
                            StartCoroutine(FixateHitAndReturnHome(0.0f));
                        }
                        else
                        {
                            if (Vector3.Distance(transform.position, m_hookBase.position) < 7f)
                            {
                                StartCoroutine(FixateHitAndReturnHome(0.0f));
                            }
                        }
                        break;
                    }

            }
        }
        else
        {
            StartCoroutine(FixateHitAndReturnHome(0.0f));
        }
    }

    public IEnumerator FixateHitAndReturnHome(float delay)
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
        transform.parent = null;
        transform.parent = m_parent;
        transform.localScale = m_defaultHookScale;
        m_handMesh.localScale = Vector3.one;

        m_returnableObject = ReturnableObject.None;
    }

    public void ResetTransform()
    {
        transform.localPosition = new Vector3(0f, 0f, 0.63f);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
