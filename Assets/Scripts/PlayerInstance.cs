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

    [HideInInspector] public bool m_isAlive;


    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;

    private string m_animationRunName = "Run";
    private float m_normalizeTimeDelay = 0f;

    private bool m_canRun;

    private bool m_isSlowmoEnable;

    private Coroutine m_normalizeTime;


    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
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
                if (!m_isSlowmoEnable)
                {
                    EnableSlowmo(true);
                    StartCoroutine(m_gun.EnableLaserSight(true, 0.01f));
                    StartCoroutine(ChangeSlowmoLocalState(true, 0f));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(ChangeSlowmoLocalState(false, m_normalizeTimeDelay));
                EnableSlowmo(false);
                StartCoroutine(m_gun.EnableLaserSight(false, 0f));
            }


            if (m_selfRigidbody.velocity.y < -2f)
            {
                print(m_selfRigidbody.velocity.y);

                ChangeLifeState(false);
                AllowToRun(false);
            }
        }
    }

    private void AllowToRun(bool state)
    {
        m_canRun = state;
    }

    private void ChangeLifeState(bool state)
    {
        m_isAlive = state;
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
}
