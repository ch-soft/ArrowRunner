using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Animator m_animator;
    [BoxGroup("References"), SerializeField] private Animator m_swordAnimator;
    [BoxGroup("References"), SerializeField] private Rigidbody[] m_bonesRigidbodies;
    [BoxGroup("References"), SerializeField] private BoxCollider m_boxCollider;
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;

    [Space]

    [BoxGroup("Preferences"), SerializeField] private Color m_deathColor;

    [HideInInspector] public bool m_isAlive;

    private string m_prapareWeaponAnimName = "PrepareAxe"; //we will use this later
    private string m_punchAnimName; //we will use this later

    private bool m_enableDeathColor;

    private void Awake()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void Start()
    {
        ChangeAliveState(true);
    }

    private void ChangeAliveState(bool state)
    {
        m_isAlive = state;
    }

    public void OnHookGrab()
    {
        StartCoroutine(GrabCharacter());
    }

    private IEnumerator GrabCharacter()
    {
        StartCoroutine(EnableBoxCollider(0.0f, false));
        ActivateRagdoll();
        EnableAnimator(false);
        StartCoroutine(PullObjectToPlayer());
        FixateDeath("Hook");
        ChangeLayers();
        yield return new WaitForSeconds(0.15f);

    }

    private void Update()
    {
        if (!m_isAlive)
        {
            ChangeColorDueLifeState();
        }
    }

    private void ActivateRagdoll()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;

            m_bonesRigidbodies[i].useGravity = true;
        }
    }

    private void ChangeLayers()
    {
        m_rigidbody.gameObject.layer = 2;

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].gameObject.layer = 2;
        }
    }

    private void EnableAnimator(bool state)
    {
        m_animator.enabled = state;

    }

    private IEnumerator EnableBoxCollider(float delay, bool state)
    {
        yield return new WaitForSeconds(delay);

        m_boxCollider.enabled = state;
    }

    private void FixateDeath(string reason)
    {
        ChangeAliveState(false);

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].gameObject.layer = 1;
        }

        StartCoroutine(EnableBoxCollider(0.1f, false));
        m_rigidbody.isKinematic = true;

        switch (reason)
        {
            case "Bridge":
                {
                    m_animator.Play("DeathFromBridge");

                    break;
                }
            case "Hook":
                {

                    break;
                }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Bridge":
                {
                    GrabbingBridge bridge = collision.gameObject.GetComponent<GrabbingBridge>();
                    if (bridge.m_isFalling)
                    {
                        FixateDeath("Bridge");
                    }

                    break;
                }
        }
    }

    private void ChangeColorDueLifeState()
    {
        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
        {
            m_selfRenderer.materials[i].color = Color.Lerp(m_selfRenderer.material.color, m_deathColor, Time.fixedDeltaTime * 4f);
        }
    }

    public void PlayPrepareWeaponAnim()
    {
        m_animator.Play(m_prapareWeaponAnimName);
        m_swordAnimator.Play("GetSwordFromBack");
    }
}
