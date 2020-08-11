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
}
