using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GrapplingBase : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Renderer[] m_selfRenderers;
    [BoxGroup("Preferences"), SerializeField] private Color m_defaultColor;

    private bool m_isOutlineActive;
    [HideInInspector] public bool m_isAlive;
    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;
    private float m_distanceToCharacter;

    private float m_timeInAir;
    private float m_animationSpeed;

    private void Start()
    {
        m_disabledMaterial = m_selfRenderers[0].material;
    }

    public void OnHookGrab()
    {
        m_distanceToCharacter = Vector3.Distance(m_playerInstance.transform.position, transform.position);
        if ((m_distanceToCharacter >= 0) && (m_distanceToCharacter < 10))
        {
            m_timeInAir = 0.4f;
            m_animationSpeed = 1.1f;

        }
        else if ((m_distanceToCharacter >= 10) && (m_distanceToCharacter < 14))
        {
            m_timeInAir = 0.45f;
            m_animationSpeed = 0.9f;

        }
        else if ((m_distanceToCharacter >= 14) && (m_distanceToCharacter < 17))
        {
            m_timeInAir = 0.5f;
            m_animationSpeed = 0.8f;

        }
        else if ((m_distanceToCharacter >= 17) && (m_distanceToCharacter < 100))
        {
            m_timeInAir = 0.6f;
            m_animationSpeed = 0.7f;
        }

        StartCoroutine(MakeGrapplingMove(m_timeInAir, m_animationSpeed));
        //gameObject.layer = 16;
    }

    public void SwitchOutlineState(bool state)
    {
        switch (state)
        {
            case true:
                {
                    if (!m_isOutlineActive && !m_objectWasAttracted)
                    {
                        for (int i = 0; i < m_selfRenderers.Length; i++)
                        {

                            m_selfRenderers[i].material = m_activeMaterial;
                            m_selfRenderers[i].material.color = m_defaultColor;
                        }

                        //m_localMaterials = m_activeMaterial;
                        m_isOutlineActive = true;
                    }
                    break;
                }
            case false:
                {
                    if (m_isOutlineActive)
                    {
                        for (int i = 0; i < m_selfRenderers.Length; i++)
                        {
                            m_selfRenderers[i].material = m_disabledMaterial;
                        }

                        //m_localMaterials = m_disabledMaterial;
                        m_isOutlineActive = false;
                    }
                    break;
                }
        }
    }
}
