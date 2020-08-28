using System.Collections;
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
    [BoxGroup("References"), SerializeField] private Collider[] m_bonesColliders;
    [BoxGroup("References"), SerializeField] private Transform m_characterRig;
    [BoxGroup("References"), SerializeField] private LevelController m_levelController;
    [BoxGroup("References"), SerializeField] private Animation m_flipAnimation;
    [HideInInspector] public CameraController m_cameraController;



    [HideInInspector] public bool m_isAlive;


    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;

    private string m_animationRunName = "Run";
    private string m_animationRLevitationName = "Flying";
    private string m_animationDancingName = "WinnerDancing";
    private float m_normalizeTimeDelay = 0f;

    private bool m_canRun;
    private bool m_enableCollectVelocityInfo;
    private bool m_canShootLaserSight;
    private bool m_isSlowmoEnable;
    private bool m_isRigCentralized;

    private bool m_allowToJump;

    private Coroutine m_normalizeTime;

    private const int m_dangerousObjectLayer = 8;


    private Quaternion m_rigLocalRotation;

    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_rigLocalRotation = m_characterRig.localRotation;
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

        m_isRigCentralized = true;
    }

    private void FixedUpdate()
    {
        if (m_canRun)
        {
            MoveCharacterForward();

            if (m_allowToJump)
            {
                m_selfRigidbody.velocity += new Vector3(0, 0.85f, 0f) / 2f;
            }
        }
    }
    private void LateUpdate()
    {
        //if (Input.GetMouseButtonUp(0))
        //{
        //    PlayJumpOverEnemyAnimation();
        //}

        if (m_isAlive && !TimeControl.m_levelFinished)
        {
            if (!m_isRigCentralized)
            {
                m_characterRig.localPosition = Vector3.Lerp(m_characterRig.localPosition, Vector3.zero, Time.deltaTime * 100f);
            }

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
                if (m_selfRigidbody.velocity.y < -3.25f)
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
        m_levelController.OpenDefeatPanel();
        TimeControl.m_levelFinished = true;
        m_cameraController.FreeFromParent();
    }

    public void AllowToRun(bool state)
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

    public void EnableFreeJump(bool state, float delay)
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
                    StartCoroutine(ForbitToFreeJump(delay));
                    break;
                }
        }


    }

    private void AllowToFreeJump()
    {
        EnableToCollectVelocityInfo(false);
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            //m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;

            m_bonesRigidbodies[i].useGravity = false;
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.None;
        }
        m_cameraController.EnableFreeCamera(true);
    }

    private IEnumerator ForbitToFreeJump(float delay)
    {
        m_isRigCentralized = false;

        ////m_selfRigidbody.useGravity = true;

        yield return new WaitForSeconds(delay);
        m_characterRig.localRotation = m_rigLocalRotation;
        m_selfAnimator.speed = 1f;
        StartRunAnimation();

        //m_gun.m_hook.ResetDefaultHookParapemers();
        yield return new WaitForSeconds(1.0f);
        m_cameraController.EnableFreeCamera(false);
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].useGravity = true;
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.Interpolate;
        }

        //m_selfRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;

        //for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        //{
        //    m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
        //}
        yield return new WaitForSeconds(1f);
        m_isRigCentralized = true;
        EnableToCollectVelocityInfo(true);
    }

    private void MoveCharacterForward()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward, m_movementSpeed * Time.fixedDeltaTime); ; /*m_selfRigidbody.MovePosition(transform.position + Vector3.forward * m_movementSpeed * Time.fixedDeltaTime);*/
    }

    public void EnableRigidbodyJump(float animaSpeed)
    {
        PlayFlipAnimation(animaSpeed);
        m_allowToJump = true;
        EnableFreeJump(true, 0f);
    }

    public void DisableRigidbodyJump(float delay)
    {
        m_allowToJump = false;
        EnableFreeJump(false, delay);
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


                    for (int i = 0; i < m_bonesColliders.Length; i++)
                    {
                        m_bonesColliders[i].enabled = true;
                    }

                    for (int i = 0; i < m_bonesRigidbodies.Length; i++)
                    {
                        m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;
                        m_bonesRigidbodies[i].isKinematic = false;

                        m_bonesRigidbodies[i].useGravity = true;
                    }
                    break;
                }
            case false:
                {

                    for (int i = 0; i < m_bonesColliders.Length; i++)
                    {
                        m_bonesColliders[i].enabled = false;
                    }

                    for (int i = 0; i < m_bonesRigidbodies.Length; i++)
                    {
                        //m_bonesRigidbodies[i].constraints = RigidbodyConstraints.FreezeRotation;
                        m_bonesRigidbodies[i].isKinematic = true;

                        m_bonesRigidbodies[i].useGravity = false;
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

    public IEnumerator PlayDancingAnimation()
    {
        m_selfAnimator.Play(m_animationDancingName);

        m_selfRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_selfRigidbody.interpolation = RigidbodyInterpolation.None;

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.None;
        }

        EnableRagdoll(false);

        m_cameraController.FreeFromParent();
        m_cameraController.ResetPosition();
        yield return new WaitForSeconds(0.3f);
        m_cameraController.m_rotateAroundCharacter = true;

    }

    private void PlayFlipAnimation(float animSpeed)
    {
        m_selfAnimator.speed = animSpeed;
        //m_selfAnimator.Play("SwingToLand");
        m_selfAnimator.Play("BackFlip");
        //Animation backFlipAnim = m_selfAnimator["BackFlip"];
    }

    private void PlayRopeJumpAnimation()
    {
        m_selfAnimator.Play("RopeJump");
    }

    private void PlayJumpOverEnemyAnimation()
    {
        m_selfAnimator.Play("OnEnemyJump_P1");
    }
}
