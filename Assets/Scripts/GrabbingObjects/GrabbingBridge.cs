using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingBridge : GrabbingBaseObject, IOnHookGrab
{
    [HideInInspector] public bool m_isFalling;

    private void Awake()
    {

    }

    public void OnHookGrab()
    {
        m_isFalling = true;
        PullBridge();
    }
}
