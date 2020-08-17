using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerInstance : MonoBehaviour
{
    [BoxGroup("Preferences"), SerializeField] private float m_movementSpeed;
    [Space]
    [BoxGroup("References"), SerializeField] private GunInstance m_gun;
    [BoxGroup("References"), SerializeField] private Rigidbody[] m_bonesRigidbodies;
    [BoxGroup("References"), SerializeField] private CameraController m_cameraController;
    [BoxGroup("References"), SerializeField] private LevelController m_levelController;

    [HideInInspector] public bool m_isAlive;


    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;

    private string m_animationRunName = "Run";
    private string m_animationRLevitationName = "Flying";
    private float m_normalizeTimeDelay = 0f;

    private bool m_canRun;
    private bool m_enableCollectVelocityInfo;

    private bool m_canShootLaserSight;

    private bool m_isSlowmoEnable;

    private Coroutine m_normalizeTime;

    private const int m_dangerousObjectLayer = 8;

    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(EnableShootLaserSight(true, 0f));

        EnableRagdoll(false);
        EnableToCollectVelocityInfo(true);

        //EnableSlowmo(false);
        ChangeLifeState(true);

        StartRunAnimation();
        AllowToRun(true);
    }

    private void FixedUpdate()
    {
        if (m_canRun)
        {
            MoveCharacterForward();
        }
    }
    private void LateUpdate()
    {
        if (m_isAlive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_canShootLaserSight)
                {
                    StartCoroutine(EnableShootLaserSight(false, 0f));
                    if (!m_isSlowmoEnable)
                    {
                        EnableSlowmo(true);
                        StartCoroutine(m_gun.EnableLaserSight(true, 0.01f));
                        StartCoroutine(ChangeSlowmoLocalState(true, 0f));
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(EnableShootLaserSight(true, 0f));
                StartCoroutine(ChangeSlowmoLocalState(false, m_normalizeTimeDelay));
                EnableSlowmo(false);
                StartCoroutine(m_gun.EnableLaserSight(false, 0f));
            }

            if (m_enableCollectVelocityInfo)
            {
                if (m_selfRigidbody.velocity.y < -2.6f)
                {
                    print(m_selfRigidbody.velocity.y); //this is for tests, need delete later

                    FixateDeath();
                }
            }
        }
    }

    private void FixateDeath()
    {
        ChangeLifeState(false);
        AllowToRun(false);
        EnableRagdoll(true);
        m_levelController.OpenEndgamePanel();
    }

    private void AllowToRun(bool state)
    {
        m_canRun = state;
    }

    private void ChangeLifeState(bool state)
    {
        m_isAlive = state;
        TimeControl.m_characterIsAlive = state;
    }

    private void EnableToCollectVelocityInfo(bool state)
    {
        m_enableCollectVelocityInfo = state;
    }

    public void EnableFreeJump(bool state)
    {

        switch (state)
        {
            case true:
                {
                    AllowToFreeJump();
                    break;
                }

            case false:
                {
                    StartCoroutine(ForbitToFreeJump());
                    break;
                }
        }


    }

    private void AllowToFreeJump()
    {
        AllowToRun(false);
        EnableToCollectVelocityInfo(false);
        m_selfRigidbody.useGravity = false;
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].useGravity = false;
        }
        m_cameraController.EnableFreeCamera(true);

        m_selfAnimator.enabled = false;

    }

    private IEnumerator ForbitToFreeJump()
    {
        AllowToRun(true);
        m_selfRigidbody.useGravity = true;
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].useGravity = true;
        }
        yield return new WaitForSeconds(1f);
        m_selfAnimator.enabled = true;

        yield return new WaitForSeconds(1f);
        m_cameraController.EnableFreeCamera(false);
        EnableToCollectVelocityInfo(true);

    }

    private void MoveCharacterForward()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward, m_movementSpeed * Time.fixedDeltaTime); ; /*m_selfRigidbody.MovePosition(transform.position + Vector3.forward * m_movementSpeed * Time.fixedDeltaTime);*/
    }

    private void StartRunAnimation()
    {
        m_selfAnimator.Play(m_animationRunName);
    }

    private void EnableSlowmo(bool state)
    {
        switch (state)
        {
            case true:
                {
                    if (!m_isSlowmoEnable)
                    {
                        TimeControl.SlowTime();
                        if (m_normalizeTime != null)
                        {
                            StopCoroutine(m_normalizeTime);
                        }
                    }
                    break;
                }
            case false:
                {
                    m_normalizeTime = StartCoroutine(TimeControl.NormalizeTime(m_normalizeTimeDelay));
                    break;
                }
        }
    }

    private IEnumerator ChangeSlowmoLocalState(bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        m_isSlowmoEnable = state;
    }

    private IEnumerator EnableShootLaserSight(bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        m_canShootLaserSight = state;
    }

    private void EnableRagdoll(bool state)
    {
        switch (state)
        {
            case true:
                {
                    m_selfAnimator.enabled = false;

                    for (int i = 0; i < m_bonesRigidbodies.Length; i++)
                    {
                        m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;
                        m_bonesRigidbodies[i].useGravity = true;
                    }
                    break;
                }
            case false:
                {
                    for (int i = 0; i < m_bonesRigidbodies.Length; i++)
                    {
                        m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeRotation;
                        m_bonesRigidbodies[i].useGravity = true;
                    }

                    break;
                }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            case m_dangerousObjectLayer:
                {
                    FixateDeath();
                    break;
                }
        }
    }

    public void PlayJumpAnumation()
    {
        m_selfAnimator.Play("JumpOnLiana");
    }
}
