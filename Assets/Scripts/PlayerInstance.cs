using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerInstance : MonoBehaviour
{
    [BoxGroup("Preferences"), SerializeField] private float m_movementSpeed;

    private Rigidbody m_selfRigidbody;
    private Animator m_selfAnimator;

    private string m_animationRunName = "Run";

    private bool m_canRun;

    private void Awake()
    {
        m_selfRigidbody = GetComponent<Rigidbody>();
        m_selfAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
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
}
