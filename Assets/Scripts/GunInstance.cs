using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(LineRenderer))]
public class GunInstance : MonoBehaviour
{
    [BoxGroup("Gun Parameters"), SerializeField] private float m_laserDistance;

    [BoxGroup("References"), SerializeField] private Camera m_mainCamera;
    [BoxGroup("References"), SerializeField] private Transform m_rightHand;



    private LineRenderer m_lineRenderer;

    private Vector3 m_laserEndPosition;

    private bool m_laserActivityState;

    private Vector3 m_testMousePosition;
    private Vector3 m_startingMousePosition;

    private float m_deltaX;
    private float m_deltaY;

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
        if (Input.GetMouseButtonDown(0))
        {
            m_startingMousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);

            m_deltaX = m_startingMousePosition.x - transform.position.x;
            m_deltaY = m_startingMousePosition.y - transform.position.y;
        }

        if (m_laserActivityState)
        {
            ControlLaserEndPosition();
            ShootLaserFromGun();
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

    private void ShootLaserFromGun()
    {
        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, m_laserEndPosition);
    }

    private void ControlLaserEndPosition()
    {
        m_testMousePosition = Input.mousePosition;
        m_testMousePosition.z = 1.0f;

        m_startingMousePosition = m_mainCamera.ScreenToWorldPoint(m_testMousePosition);
        //m_laserEndPosition = new Vector3(m_startingMousePosition.x - m_deltaX, m_startingMousePosition.y - m_deltaY, transform.position.z + 20f);
        m_laserEndPosition = m_startingMousePosition;
    }




    //m_laserEndPosition = transform.position + (Vector3.forward * m_laserDistance); //just shoot forward
}
