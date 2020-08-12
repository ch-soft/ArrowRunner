using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Animator m_animator;
    [BoxGroup("References"), SerializeField] private Rigidbody[] m_bonesRigidbodies;
    [BoxGroup("References"), SerializeField] private BoxCollider m_boxCollider;
    [Space]
    [BoxGroup("Preferences"), SerializeField] private string m_punchAnimName;

    [HideInInspector] public bool m_isAlive;

    private void Awake()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void OnHookGrab()
    {
        switch (m_grabbingObjectType)
        {
            case GrabbingObjectType.EnemyCharacter:
            case GrabbingObjectType.EnvironmentObject:
                {
                    StartCoroutine(PullObjectToPlayer());
                    EnableAnimator(false);
                    ActivateRagdoll();
                    break;
                }
        }
    }

    private void ActivateRagdoll()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;

            m_bonesRigidbodies[i].useGravity = true;
        }
    }

    private void EnableAnimator(bool state)
    {
        m_animator.enabled = state;

        m_boxCollider.enabled = state;
    }

    private void FixateDeath(string reason)
    {
        switch (reason)
        {
            case "Bridge":
                {
                    m_isAlive = false;
                    m_animator.Play("DeathFromBridge");
                    m_boxCollider.enabled = false;

                    m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                    break;
                }
            case "Hook":
                {

                    break;
                }

        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Bridge":
                {
                    GrabbingBridge bridge = collision.gameObject.GetComponent<GrabbingBridge>();
                    if (bridge.m_isFalling)
                    {
                        FixateDeath("Bridge");
                    }

                    break;
                }
        }
    }
}
