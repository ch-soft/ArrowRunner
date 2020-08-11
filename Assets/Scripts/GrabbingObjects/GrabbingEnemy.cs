using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Animator m_animator;
    [BoxGroup("References"), SerializeField] private Rigidbody[] m_bonesRigidbodies;
    [Space]
    [BoxGroup("Preferences"), SerializeField] private string m_punchAnimName;




    public void OnHookGrab()
    {
        switch (m_grabbingObjectType)
        {
            case GrabbingObjectType.EnemyCharacter:
            case GrabbingObjectType.EnvironmentObject:
                {
                    StartCoroutine(PullObjectToPlayer());
                    ActivateRagdoll();
                    break;
                }
        }
    }

    private void ActivateRagdoll()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].useGravity = true;
        }
    }
}
