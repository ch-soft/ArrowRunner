using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class GrabbingEnemy : GrabbingBaseObject, IOnHookGrab
{
    [BoxGroup("References"), SerializeField] private Animator m_animator;
    [BoxGroup("References"), SerializeField] private Animator m_swordAnimator;
    [BoxGroup("References"), SerializeField] private Rigidbody[] m_bonesRigidbodies;
    [BoxGroup("References"), SerializeField] private Collider m_collider;
    [BoxGroup("References"), SerializeField] private SkinnedMeshRenderer m_selfRenderer;
    [BoxGroup("References"), SerializeField] private Rigidbody m_headRigidbody;
    [BoxGroup("References"), SerializeField] private EnemyDistanceController m_distanceController;
    [BoxGroup("References"), SerializeField] private GameObject m_enemyRig;
    [BoxGroup("References"), SerializeField] private Mesh[] m_meshesList;

    [Space]
    [BoxGroup("Settings"), SerializeField] private Color m_deathColor;
    [Space]
    [BoxGroup("Settings"), SerializeField] private Material[] m_activeMaterials;
    [BoxGroup("Settings")] public DeathType m_deathType;

    [HideInInspector] public bool m_isAlive;

    private Material[] m_disabledMaterials;
    private Material[] m_localMaterials;

    private string m_prapareWeaponAnimName = "GetAxeFromBack"; //we will use this later
    private string m_punchAnimName; //we will use this later

    private float m_headPunchForce = 15f;
    private float m_horizontalX;

    private bool m_enableDeathColor;
    private bool m_isOutlineActive;
    private bool m_canFlyToPlayer;

    private bool m_isExplosionDeath;

    private void Start()
    {
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
        }

        m_disabledMaterials = m_selfRenderer.materials;

        ChangeAliveState(true);

        m_horizontalX = transform.position.x * 10f;

        m_selfRenderer.sharedMesh = m_meshesList[Random.Range(0, m_meshesList.Length)];
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
        yield return new WaitForSeconds(0.0f);
        if (TimeControl.m_characterIsAlive)
        {
            StartCoroutine(PullObjectToPlayer());
            FixateDeath("Hook");
            StartCoroutine(PushEnemyToPlayer());
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
        if (m_enableDeathColor)
        {
            ChangeColorDueLifeState();

            if (m_headPunchForce > 0f)
            {
                m_headRigidbody.velocity += new Vector3(m_horizontalX, m_headPunchForce, m_headPunchForce * 1.5f);
                m_headPunchForce--;
            }
        }

        if (m_canFlyToPlayer)
        {
            //m_rigidbody.MovePosition(transform.position + m_playerInstance.transform.position);

            m_rigidbody.velocity = (m_playerInstance.transform.position - transform.position) * Time.deltaTime * 700f;

            //m_rigidbody.velocity -= m_playerInstance.transform.position / 10f;
            //transform.position = Vector3.Lerp(transform.position, m_playerInstance.transform.position, Time.deltaTime * 5f);
        }
    }

    private void ActivateRagdoll()
    {
        m_enemyRig.SetActive(true);

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;

            m_bonesRigidbodies[i].useGravity = true;
        }
    }

    private void ChangeLayers(int newLayer)
    {
        m_rigidbody.gameObject.layer = newLayer;

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].gameObject.layer = newLayer;
        }
    }

    private void EnableAnimator(bool state)
    {
        m_animator.enabled = state;
    }

    private IEnumerator EnableBoxCollider(float delay, bool state)
    {
        yield return new WaitForSeconds(delay);

        m_collider.enabled = state;
    }

    private void FixateDeath(string reason)
    {
        ChangeAliveState(false);
        SwitchOutlineWtate(false);
        //for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        //{
        //    m_bonesRigidbodies[i].gameObject.layer = 16;
        //}

        //m_rigidbody.isKinematic = true;

        switch (reason)
        {
            case "Bridge":
                {
                    m_animator.Play("DeathFromBridge");
                    ChangeLayers(16);
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
                    m_playerInstance.PlayPunchParticles(m_headRigidbody.transform.position);
                    PushEnemyBack();
                    break;
                }
        }
    }

    public void PushEnemyByExplosion()
    {
        m_canFlyToPlayer = false;
        EnableAnimator(false);
        m_collider.enabled = false;
        ActivateRagdoll();
        ChangeLayers(2);
        m_enableDeathColor = true;
        m_distanceController.gameObject.SetActive(false);
        m_playerInstance.ShowCoolWord();

        m_playerInstance.NormalizeSpeedAndTime(); //type 1
    }

    public void PushEnemyBack()
    {
        if ((m_playerInstance.m_isAlive))
        {
            switch (m_deathType)
            {
                case DeathType.Forward:
                    {
                        m_canFlyToPlayer = false;
                        EnableAnimator(false);
                        m_collider.enabled = false;
                        ActivateRagdoll();
                        ChangeLayers(2);
                        m_enableDeathColor = true;
                        m_distanceController.gameObject.SetActive(false);
                        m_playerInstance.ShowCoolWord();

                        m_playerInstance.NormalizeSpeedAndTime(); //type 1
                        break;
                    }
                case DeathType.Down:
                    {
                        m_canFlyToPlayer = false;
                        EnableAnimator(false);
                        m_collider.enabled = false;
                        //ActivateRagdoll();
                        ChangeLayers(2);
                        m_enableDeathColor = true;
                        m_distanceController.gameObject.SetActive(false);
                        m_playerInstance.ShowCoolWord();

                        m_rigidbody.velocity += m_rigidbody.velocity + Vector3.down * 10f;

                        StartCoroutine(m_playerInstance.PlayJumpOverAnimation()); //type 1
                        break;
                    }

            }
        }
    }

    private IEnumerator PushEnemyToPlayer()
    {
        switch (m_deathType)
        {
            case DeathType.Forward:
                {
                    PlayGrabbingPoseAnim();
                    m_enemyRig.SetActive(false);
                    m_distanceController.gameObject.SetActive(false);
                    m_canFlyToPlayer = true;
                    break;
                }

            case DeathType.Down:
                {
                    PlayGrabbingPoseAnim();
                    m_enemyRig.SetActive(false);
                    m_distanceController.gameObject.SetActive(false);
                    m_canFlyToPlayer = true;
                    break;
                }
        }



        yield return new WaitForSecondsRealtime(0.2f);
        EnableAnimator(false);
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
                        m_localMaterials = m_activeMaterials;

                        m_isOutlineActive = true;
                    }
                    break;
                }
            case false:
                {
                    if (m_isOutlineActive)
                    {

                        m_localMaterials = m_disabledMaterials;

                        m_isOutlineActive = false;
                    }
                    break;
                }
        }
        m_selfRenderer.materials = m_localMaterials;
    }

    private void PlayGrabbingPoseAnim()
    {
        m_animator.Play("GrabbingPose");
    }
}
