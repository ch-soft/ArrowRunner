using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(LineRenderer))]
public class GunInstance : MonoBehaviour
{
    [BoxGroup("Gun Parameters"), SerializeField] private float m_laserDistance;
    [BoxGroup("References"), SerializeField] private Transform m_rightHand;



    private LineRenderer m_lineRenderer;

    private bool m_laserActivityState;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        StartingSetup();
    }

    private void Update()
    {
        if (m_laserActivityState)
        {
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, transform.position + (Vector3.forward * m_laserDistance));
        }
    }

    private void StartingSetup()
    {
        transform.SetParent(m_rightHand);
    }

    public void EnableLaserSight(bool state)
    {
        m_laserActivityState = state;
        m_lineRenderer.enabled = state;
    }
}
