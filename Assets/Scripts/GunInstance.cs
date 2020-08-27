using System.Collections;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(LineRenderer))]
public class GunInstance : MonoBehaviour
{

    //[BoxGroup("References"), SerializeField] private Camera m_mainCamera;
    [BoxGroup("References"), SerializeField] private Transform m_rightHand;
    [BoxGroup("References")] public HookInstance m_hook;
    [BoxGroup("References"), SerializeField] private GameObject m_pointSphere;


    private LineRenderer m_lineRenderer;
    private CameraController m_cameraController;
    private Camera m_camera;

    private Vector3 m_laserEndPosition;
    private Vector3 m_secondaryLaserEndPosition;
    private Vector3 m_testMousePosition;
    private Vector3 m_currentgMousePosition;
    private Vector3 m_startingMousePosition;

    private Vector3 m_defaultGunPosition;

    private float m_sensitivity = 45f;

    private float m_laserDistance = 25f;

    private bool m_laserActivityState;


    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_camera = Camera.main;
    }

    private void Start()
    {
        StartingSetup();
    }
    private void StartingSetup()
    {
        transform.SetParent(m_rightHand);

        m_defaultGunPosition = transform.position;
    }

    private void Update()
    {
        if (!TimeControl.m_levelFinished)
        {
            if (Input.GetMouseButtonDown(0))
            {

                m_testMousePosition = Input.mousePosition;
                m_testMousePosition.z = 1.0f;

                Vector3 oneMoreValue = m_camera.ScreenToWorldPoint(m_camera.WorldToScreenPoint(new Vector3(0f, m_cameraController.m_HeightDifference().y, 0f)));

                m_startingMousePosition = m_camera.ScreenToWorldPoint(m_testMousePosition) - oneMoreValue;
            }

            if (m_laserActivityState)
            {
                RaycastHit hit;

                m_testMousePosition = Input.mousePosition;
                m_testMousePosition.z = 1.0f;
                Vector3 oneMoreValue = m_camera.ScreenToWorldPoint(m_camera.WorldToScreenPoint(new Vector3(0f, m_cameraController.m_HeightDifference().y, 0f)));

                m_currentgMousePosition =
                    (m_camera.ScreenToWorldPoint(m_testMousePosition) - oneMoreValue - m_startingMousePosition) * m_sensitivity;
                m_currentgMousePosition.z = 0f;
                m_currentgMousePosition.x = 0f;

                m_secondaryLaserEndPosition = /*transform.position + */new Vector3(0f, 0f, m_laserDistance) + m_currentgMousePosition;
                if (Physics.Raycast(transform.position, m_secondaryLaserEndPosition, out hit, m_laserDistance))
                {
                    Debug.DrawRay(transform.position, m_secondaryLaserEndPosition);
                    m_laserEndPosition = hit.point; // this is for full controll
                    m_laserEndPosition.x = 0f;

                    m_pointSphere.transform.position = m_laserEndPosition;
                    EnablePointSphere(true);
                }
                else
                {
                    m_laserEndPosition = m_secondaryLaserEndPosition + transform.position;
                    EnablePointSphere(false);
                }

                if (!TimeControl.m_levelFinished)
                {
                    ShootLaserFromGun();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_hook.m_hookState == HookState.Based)
                {
                    ConfirmAimOnTarget();
                }
            }
        }

    }

    public IEnumerator EnableLaserSight(bool state, float delay)
    {
        m_laserActivityState = state;
        yield return new WaitForSecondsRealtime(delay);
        m_lineRenderer.enabled = state;
        EnablePointSphere(state);
    }

    private void ShootLaserFromGun()
    {
        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, m_laserEndPosition);
    }

    //m_laserEndPosition = transform.position + (Vector3.forward * m_laserDistance); //just shoot forward

    private void ConfirmAimOnTarget()
    {
        RaycastHit hit;


        m_testMousePosition = Input.mousePosition;
        m_testMousePosition.z = 1.0f;



        Vector3 oneMoreValue = m_camera.ScreenToWorldPoint(m_camera.WorldToScreenPoint(new Vector3(0f, m_cameraController.m_HeightDifference().y, 0f)));

        m_currentgMousePosition =
                (m_camera.ScreenToWorldPoint(m_testMousePosition) - oneMoreValue - m_startingMousePosition) * m_sensitivity;
        m_currentgMousePosition.z = 0f;
        m_currentgMousePosition.x = 0f;
        m_secondaryLaserEndPosition = /*transform.position + */new Vector3(0, 0, m_laserDistance) + m_currentgMousePosition;

        if (Physics.Raycast(transform.position, m_secondaryLaserEndPosition, out hit, m_laserDistance))
        {
            //if (hit.collider.gameObject.layer == m_grabbingObjectLayer)
            //{
            //enemy was found, we can shoot hook
            m_hook.m_targetPosition = new Vector3(0f, hit.point.y, hit.point.z);
            m_hook.m_hookState = HookState.FliesToTarget;
            m_hook.transform.parent = null;
            m_hook.ChangeLocalHookState(true);
            //}

        }
        else
        {
            //enemy is not is sight, hook will be short
        }

        m_startingMousePosition = Vector3.zero;
    }

    private void EnablePointSphere(bool state)
    {
        if (m_pointSphere.activeSelf == !state)
        {
            m_pointSphere.SetActive(state);
        }
    }
}
