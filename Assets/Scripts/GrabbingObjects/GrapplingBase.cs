using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GrapplingBase : GrabbingBaseObject, IOnHookGrab
{

    [HideInInspector] public Transform m_characterTransform;
    [HideInInspector] public Rigidbody m_characterRigidbody;


    public void OnHookGrab()
    {
        PrepareGrapplingMove();
    }
}
