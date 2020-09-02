using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingBarrel : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [BoxGroup("References"), SerializeField] private Transform m_endPoint;

    [HideInInspector] public bool m_isFalling;

    private bool m_isStanding;
    private float m_afterFallingDelay = 0.2f;

    private bool m_isOutlineActive;
    [HideInInspector] public bool m_isAlive;
    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;

    private bool m_enableFlyToEndpoint;

    private float m_beatingForce = 50f;

    private void Start()
    {
        m_isStanding = true;

        m_disabledMaterial = m_selfRenderer.material;
    }

    private void Update()
    {
        if (m_enableFlyToEndpoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_endPoint.position, Time.deltaTime * m_beatingForce);
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
        PullBarrel();

        DisableBarrel();
        //m_isFalling = true;

    }

    private void DisableBarrel()
    {
        gameObject.layer = 19;
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

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        switch (collision.gameObject.tag)
        {
            case "Player":
                {
                    ThrowToEndPoint();
                    PlayerInstance player = collision.gameObject.GetComponent<PlayerInstance>();
                    player.NormalizeSpeedAndTime();
                    break;
                }
        }
    }


    private void ThrowToEndPoint()
    {
        m_isGrabbing = false;

        m_enableFlyToEndpoint = true;
    }
}
