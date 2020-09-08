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
    [BoxGroup("References"), SerializeField] private CoolLettering m_coolLettering;
    [BoxGroup("References"), SerializeField] private HookInstance m_hookInstance;
    [BoxGroup("References"), SerializeField] private ParticleSystem m_punchParticles;
    [HideInInspector] public CameraController m_cameraController;

    [HideInInspector] public bool m_isAlive;
    [HideInInspector] public bool m_playerIsKnocks;

    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;
    private Quaternion m_rigLocalRotation; //old
    private IEnumerator m_checkRunAnimation;


    private string m_animationRunName = "Run";
    private string m_animationRLevitationName = "Flying";
    private string m_animationDancingName = "WinnerDancing";
    private string m_barrelKickAnimName = "BarrelKick";
    private float m_normalizeTimeDelay = 0f;

    private bool m_canRun;
    private bool m_enableCollectVelocityInfo;
    private bool m_canShootLaserSight;
    private bool m_isSlowmoEnable;
    private bool m_isRigCentralized;
    private bool m_allowToJump;

    private const int m_dangerousObjectLayer = 8;
    private float m_defaultSpeed;

    private Coroutine m_collectVelocityInfoRoutine;
    private Coroutine m_forbitJumpRoutine;

    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_rigLocalRotation = m_characterRig.localRotation;
    }

    private void Start()
    {
        EnableRagdoll(false);

        StartCoroutine(EnableShootLaserSight(true, 0f));

        m_collectVelocityInfoRoutine = StartCoroutine(EnableToCollectVelocityInfo(true, 0f));

        //EnableSlowmo(false);
        ChangeLifeState(true);
        SaveDefaultSpeed();

        PlayRunAnimation();
        AllowToRun(true);
        m_isRigCentralized = true;

        CheckPlayerTag();
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
                    print("Death velocity: " + m_selfRigidbody.velocity.y); //this is for tests, need delete later

                    FixateDeath();
                }
            }
        }
    }

    private void CheckPlayerTag()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            print("Тэг Player может быть только один!");

            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            {
                print(GameObject.FindGameObjectsWithTag("Player")[i].name);
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

    private IEnumerator EnableToCollectVelocityInfo(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
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
                    m_forbitJumpRoutine = StartCoroutine(ForbitToFreeJump(delay));
                    break;
                }
        }


    }

    private void AllowToFreeJump()
    {
        if (m_collectVelocityInfoRoutine != null)
        {
            StopCoroutine(m_collectVelocityInfoRoutine);
        }
        m_collectVelocityInfoRoutine = StartCoroutine(EnableToCollectVelocityInfo(false, 0f));
        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            //m_bonesRigidbodies[i].constraints = RigidbodyConstraints.None;

            m_bonesRigidbodies[i].useGravity = false;
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.None;
        }
        //m_cameraController.EnableFreeCamera(true);

    }

    private IEnumerator ForbitToFreeJump(float delay)
    {
        m_hookInstance.AllowReturnToBase(false);
        m_isRigCentralized = false;

        yield return new WaitForSeconds(delay);
        m_characterRig.localRotation = m_rigLocalRotation;
        m_selfAnimator.speed = 1f;
        //m_gun.m_hook.ResetTransform();
        PlayRunAnimation();

        yield return new WaitForSeconds(.5f);
        m_hookInstance.AllowReturnToBase(true);
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].useGravity = true;
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.Interpolate;
        }

        yield return new WaitForSeconds(1f);
        m_isRigCentralized = true;

        m_collectVelocityInfoRoutine = StartCoroutine(EnableToCollectVelocityInfo(true, 0f));
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

    public IEnumerator PlayKillEnemyAnimation(float distanceToPlayer)
    {
        m_playerIsKnocks = true;
        PlayJumpOverEnemyAnimation();
        StartCoroutine(CheckEnemyHitAnimationCompleted());

        ChangeSpeed(m_defaultSpeed * 3f);
        //EnableSlowmo(true);
        TimeControl.PunchSlowTime();

        float timeFromDistance;

        if ((distanceToPlayer >= 0f) && (distanceToPlayer < 4f))
        {
            m_selfAnimator.speed = 1.75f;
        }
        else if ((distanceToPlayer >= 4f) && (distanceToPlayer < 9f))
        {
            m_selfAnimator.speed = 1.2f;
        }
        else if ((distanceToPlayer >= 9f) && (distanceToPlayer < 12f))
        {
            m_selfAnimator.speed = 1f;
        }
        else if ((distanceToPlayer >= 12f) && (distanceToPlayer < 100f))
        {
            m_selfAnimator.speed = 0.65f;
        }


        yield return new WaitForSecondsRealtime(distanceToPlayer / 9f);
        ChangeSpeed(m_defaultSpeed);
        EnableSlowmo(false);

        yield return new WaitForSecondsRealtime(0.1f);
        //PlayRunAnimation();
    }

    public IEnumerator PlayKickBarrelAnimation()
    {
        TimeControl.PunchSlowTime();
        ChangeSpeed(3f);

        m_selfAnimator.speed = 2f;

        m_selfAnimator.Play(m_barrelKickAnimName);

        StartCoroutine(CheckEnemyHitAnimationCompleted());

        yield return new WaitForSecondsRealtime(0f);
    }

    public void PlayRunAnimation()
    {
        m_selfAnimator.Play(m_animationRunName);
        ResetSpeed();
    }

    private void EnableSlowmo(bool state)
    {
        if (!m_playerIsKnocks)
        {
            switch (state)
            {
                case true:
                    {
                        if (!m_isSlowmoEnable)
                        {
                            TimeControl.SlowTime();
                            //if (m_normalizeTime != null)
                            //{
                            //    StopCoroutine(m_normalizeTime);
                            //}
                        }
                        break;
                    }
                case false:
                    {
                        StartCoroutine(TimeControl.NormalizeTime(m_normalizeTimeDelay));
                        //THIS
                        break;
                    }
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
                    if (collision.gameObject.GetComponent<GrabbingEnemy>())
                    {
                        if (collision.gameObject.GetComponent<GrabbingEnemy>().m_isAlive && !m_allowToJump)
                        {
                            FixateDeath();
                        }
                    }
                    else
                    {
                        FixateDeath();
                    }
                    break;
                }
        }
    }

    public IEnumerator PlayDancingAnimation()
    {

        m_selfRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_selfRigidbody.interpolation = RigidbodyInterpolation.None;

        for (int i = 0; i < m_bonesRigidbodies.Length; i++)
        {
            m_bonesRigidbodies[i].interpolation = RigidbodyInterpolation.None;
        }

        EnableRagdoll(false);

        m_selfAnimator.enabled = false;
        m_cameraController.ChangeActiveCamera();
        //m_cameraController.ResetPosition();
        yield return new WaitForSeconds(0.1f);
        m_selfAnimator.enabled = true;
        m_selfAnimator.Play(m_animationDancingName);
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

    private void ChangeSpeed(float speed)
    {
        m_movementSpeed = speed;
    }

    private void SaveDefaultSpeed()
    {
        m_defaultSpeed = m_movementSpeed;
    }

    public void ShowCoolWord()
    {
        StartCoroutine(m_coolLettering.ShowCoolWord(0.0f)); ;
    }

    public void NormalizeSpeedAndTime()
    {
        if (!m_allowToJump)
        {
            ChangeSpeed(1f);
            StartCoroutine(TimeControl.NormalizeTime(0.3f));
            m_playerIsKnocks = false;
            StartCoroutine(m_hookInstance.FixateHitAndReturnHome(0f));
            m_selfAnimator.speed = 1f;

            m_cameraController.AllowToReturnCamera(true);
        }
    }

    public void ResetSpeed()
    {
        ChangeSpeed(m_defaultSpeed);
    }

    public void PlayPunchParticles(Vector3 collisionContactPoint)
    {
        m_punchParticles.transform.position = collisionContactPoint;
        m_punchParticles.Play();
    }

    public IEnumerator CheckEnemyHitAnimationCompleted()
    {
        while (m_selfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.95f)
        {
            yield return null;
        }
        PlayRunAnimation();
    }

    public IEnumerator PlayJumpOverAnimation()
    {
        if (m_collectVelocityInfoRoutine != null)
        {
            StopCoroutine(m_collectVelocityInfoRoutine);
        }
        if (m_forbitJumpRoutine != null)
        {
            StopCoroutine(m_forbitJumpRoutine);
        }

        m_collectVelocityInfoRoutine = StartCoroutine(EnableToCollectVelocityInfo(false, 0f));
        m_selfAnimator.Play("JumpingDown");
        transform.position += new Vector3(0f, 0.7f, 0f);
        m_selfRigidbody.velocity += new Vector3(0f, 6.5f, 0f);

        //m_allowToJump = true;
        ChangeSpeed(12f);
        StartCoroutine(TimeControl.NormalizeTime(0.15f));
        m_playerIsKnocks = false;
        StartCoroutine(m_hookInstance.FixateHitAndReturnHome(0.45f));
        m_selfAnimator.speed = 1f;

        StartCoroutine(CheckEnemyHitAnimationCompleted());

        m_collectVelocityInfoRoutine = StartCoroutine(EnableToCollectVelocityInfo(true, 2f));

        yield return new WaitForSecondsRealtime(0.4f);
        //m_allowToJump = false;
    }
}
