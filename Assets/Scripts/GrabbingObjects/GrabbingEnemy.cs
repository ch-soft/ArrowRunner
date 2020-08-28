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
    [BoxGroup("References"), SerializeField] private GameObject m_enemyRig;

    [Space]

    [BoxGroup("Preferences"), SerializeField] private Color m_deathColor;

    [Space]

    [BoxGroup("Preferences"), SerializeField] private Material m_activeMaterial;
    private Material m_disabledMaterial;
    private Material[] m_localMaterials;
    private bool m_isOutlineActive;


    [HideInInspector] public bool m_isAlive;



    private string m_prapareWeaponAnimName = "PrepareAxe"; //we will use this later
    private string m_punchAnimName; //we will use this later

    private bool m_enableDeathColor;

    private void Start()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
        }

        m_disabledMaterial = m_selfRenderer.material;

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
        yield return new WaitForSeconds(0.15f);
        if (TimeControl.m_characterIsAlive)
        {

            StartCoroutine(PullObjectToPlayer());
        }
        else
        {
            m_animator.Play("Idle_0");
            m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            for (int i = 0; i < m_bonesRigidbodies.Length; i++)
            {
                m_bonesRigidbodies[i].isKinematic = true;
            }
        }


        //StartCoroutine(EnableBoxCollider(0.0f, false));
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
        m_rigidbody.gameObject.layer = 16;

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].gameObject.layer = 16;
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
        SwitchOutlineWtate(false);
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].gameObject.layer = 16;
        }

        StartCoroutine(EnableBoxCollider(0.1f, false));
        m_rigidbody.isKinematic = true;

        switch (reason)
        {
            case "Bridge":
                {
                    m_animator.Play("DeathFromBridge");
                    ChangeLayers();
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

            case "Player":
                {
                    //if (!m_isAlive)
                    //{
                    FixateDeath("Hook");
                    ActivateRagdoll();
                    EnableAnimator(false);
                    ChangeLayers();
                    ChangeAliveState(false);
                    //}
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

    public void PlaySwordSlashAnim()
    {
        m_animator.Play("SwordSlash");
        m_rigidbody.isKinematic = true;
        //m_enemyRig.SetActive(false);
    }

    public void SwitchOutlineWtate(bool state)
    {
        m_localMaterials = m_selfRenderer.materials;
        switch (state)
        {
            case true:
                {
                    if (!m_isOutlineActive && m_isAlive)
                    {
                        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
                        {
                            m_localMaterials[i] = m_activeMaterial;
                        }
                        m_isOutlineActive = true;
                    }
                    break;
                }
            case false:
                {
                    if (m_isOutlineActive)
                    {
                        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
                        {
                            m_localMaterials[i] = m_disabledMaterial;
                        }
                        m_isOutlineActive = false;
                    }
                    break;
                }
        }
        m_selfRenderer.materials = m_localMaterials;
    }
}
