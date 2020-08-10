using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{


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
