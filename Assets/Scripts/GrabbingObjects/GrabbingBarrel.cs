using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingBarrel : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [BoxGroup("References"), SerializeField] private Transform m_endPoint;
    [BoxGroup("References"), SerializeField] private ParticleSystem m_explosionParticles;


    [HideInInspector] public bool m_isFalling;

    private bool m_isStanding;
    private float m_afterFallingDelay = 0.2f;

    private bool m_isOutlineActive;
    [HideInInspector] public bool m_isAlive;
    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;
    private bool m_enableFlyToEndpoint;
    private float m_beatingForce = 50f;
    private bool m_barrelWasBlowUp;
    private bool m_isDisintegrating;

    private const float m_findEnemiesRadius = 10f;


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
        if ((m_isDisintegrating) && (transform.localScale.x > 0.01f))
        {
            transform.localScale -= Vector3.one / 10f;
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
        //m_isFalling = true;

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
        if (!m_barrelWasBlowUp)
        {
            switch (collision.gameObject.tag)
            {
                case "Player":
                    {
                        ThrowToEndPoint();

                        break;
                    }
                case "Enemy":
                    {
                        StartCoroutine(BlowUpBarrel());
                        break;
                    }
            }
        }
    }


    private void ThrowToEndPoint()
    {
        m_isGrabbing = false;
        m_enableFlyToEndpoint = true;
        switch (collision.gameObject.tag)
        {
            case "Player":
                {
                    PlayerInstance player = collision.gameObject.GetComponent<PlayerInstance>();



                    break;
                }
        }

    }

    private IEnumerator BlowUpBarrel()
    {
        m_barrelWasBlowUp = true;
        m_isDisintegrating = true;
        m_explosionParticles.Play();
        FindNearestEnemies();
        yield return new WaitForSecondsRealtime(1f);
        m_playerInstance.PlayRunAnimation();
        m_playerInstance.NormalizeSpeedAndTime();
        m_playerInstance.ResetSpeed();
        Destroy(gameObject);
        yield return new WaitForSecondsRealtime(1f);
    }

    private void FindNearestEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Vector3 selfPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            Vector3 difference = enemy.transform.position - selfPosition;

            float currentDistance = difference.sqrMagnitude;

            if (currentDistance < m_findEnemiesRadius)
            {
                enemy.GetComponent<GrabbingEnemy>().PushEnemyBack();
            }
        }
    }
}
