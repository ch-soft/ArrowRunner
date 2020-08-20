using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GrapplingBase : GrabbingBaseObject, IOnHookGrab
{
    public void OnHookGrab()
    {
        MakeGrapplingMove();
        gameObject.layer = 16;
    }
}
