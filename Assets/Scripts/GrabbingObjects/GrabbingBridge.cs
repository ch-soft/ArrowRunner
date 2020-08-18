using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingBridge : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [BoxGroup("Preferences"), SerializeField] private Color m_standColor;

    [HideInInspector] public bool m_isFalling;

    private bool m_isStanding;
    private float m_afterFallingDelay = 0.2f;


    private void Awake()
    {
        m_isStanding = true;
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
    }

    private IEnumerator GrabCharacter()
    {
        yield return new WaitForSeconds(0.03f);
        if ((TimeControl.m_characterIsAlive))
        {
            m_isFalling = true;
            PullBridge();
            StartCoroutine(DisableBridge());

        }
        else
        {
            gameObject.layer = 0;
        }
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
}
