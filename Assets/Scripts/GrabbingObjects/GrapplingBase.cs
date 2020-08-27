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
    private float m_distanceToCharacter;
    private float m_timeInAir;

    private void Start()
    {
        m_disabledMaterial = m_selfRenderer.material;
    }

    public void OnHookGrab()
    {
        m_distanceToCharacter = Vector3.Distance(m_playerInstance.transform.position, transform.position);
        print(m_distanceToCharacter);
        if ((m_distanceToCharacter >= 0) && (m_distanceToCharacter < 10))
        {
            m_timeInAir = 0.3f;
        }
        else if ((m_distanceToCharacter >= 10) && (m_distanceToCharacter < 14))
        {
            m_timeInAir = 0.4f;
        }
        else if ((m_distanceToCharacter >= 14) && (m_distanceToCharacter < 17))
        {
            m_timeInAir = 0.4f;
        }
        else if ((m_distanceToCharacter >= 17) && (m_distanceToCharacter < 100))
        {
            m_timeInAir = 0.6f;
        }

        StartCoroutine(MakeGrapplingMove(m_timeInAir));
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
