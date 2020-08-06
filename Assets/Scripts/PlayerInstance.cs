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


    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;

    private string m_animationRunName = "Run";

    private bool m_canRun;

    private bool m_isSlowmoEnable;

    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        EnableSlowmo(false);


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
        if (Input.GetMouseButtonDown(0))
        {
            ChangeSlowmoLocalState(true);
            if (m_isSlowmoEnable)
            {
                EnableSlowmo(true);
                m_gun.EnableLaserSight(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ChangeSlowmoLocalState(false);
            EnableSlowmo(false);
            m_gun.EnableLaserSight(false);
        }
    }

    private void AllowToRun(bool state)
    {
        m_canRun = state;
    }

    private void MoveCharacterForward()
    {
        m_selfRigidbody.MovePosition(transform.position + Vector3.forward * m_movementSpeed * Time.fixedDeltaTime);
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
                    TimeControl.SlowTime();
                    break;
                }
            case false:
                {
                    TimeControl.NormalizeTime();
                    break;
                }
        }
    }

    private void ChangeSlowmoLocalState(bool state)
    {
        m_isSlowmoEnable = state;
    }
}
