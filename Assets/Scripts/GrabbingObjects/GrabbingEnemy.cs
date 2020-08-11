using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Animator m_animator;
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
                    break;
                }
        }
    }
}
