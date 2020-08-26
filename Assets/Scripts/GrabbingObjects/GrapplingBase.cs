using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GrapplingBase : GrabbingBaseObject, IOnHookGrab
{

    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [BoxGroup("Preferences"), SerializeField] private Color m_defaultColor;

    private bool m_isOutlineActive;
    [HideInInspector] public bool m_isAlive;
    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;

    private void Start()
    {
        m_disabledMaterial = m_selfRenderer.material;
    }

    public void OnHookGrab()
    {
        MakeGrapplingMove();
        //gameObject.layer = 16;
    }

    public void SwitchOutlineWtate(bool state)
    {
        switch (state)
        {
            case true:
                {
                    if (!m_isOutlineActive && !m_objectWasAttracted)
                    {
                        m_selfRenderer.material = m_activeMaterial;
                        m_selfRenderer.material.color = m_defaultColor;
                        //m_localMaterials = m_activeMaterial;
                        m_isOutlineActive = true;
                    }
                    break;
                }
            case false:
                {
                    if (m_isOutlineActive)
                    {
                        m_selfRenderer.material = m_disabledMaterial;

                        //m_localMaterials = m_disabledMaterial;
                        m_isOutlineActive = false;
                    }
                    break;
                }
        }
    }
}
