using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingBridge : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [BoxGroup("Preferences"), SerializeField] private Color m_standColor;
    [BoxGroup("Preferences"), SerializeField] private Color m_defaultColor;

    [HideInInspector] public bool m_isFalling;

    private bool m_isStanding;
    private float m_afterFallingDelay = 0.2f;

    private bool m_isOutlineActive;
    [HideInInspector] public bool m_isAlive;
    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;

    private void Start()
    {
        m_isStanding = true;

        m_disabledMaterial = m_selfRenderer.material;
    }

    private void Update()
    {
        if (!m_isStanding)
        {
            ChangeColorDueLifeState();
        }
    }


    public void OnHookGrab()
    {
        StartCoroutine(GrabCharacter());
        SwitchOutlineState(false);

    }

    private IEnumerator GrabCharacter()
    {

        yield return new WaitForSeconds(0.03f);
       
            m_isFalling = true;
            PullBridge();
            StartCoroutine(DisableBridge());

    }

    public IEnumerator DisableBridge()
    {
        yield return new WaitForSeconds(m_afterFallingDelay);
        m_isStanding = false;
        m_rigidbody.useGravity = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        gameObject.layer = 13;
    }

    private void ChangeColorDueLifeState()
    {
        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
        {
            m_selfRenderer.materials[i].color = Color.Lerp(m_selfRenderer.material.color, m_standColor, Time.fixedDeltaTime * 2f);
        }
    }

    public void SwitchOutlineState(bool state)
    {
        switch (state)
        {
            case true:
                {
                    if (!m_isOutlineActive && m_isStanding)
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
