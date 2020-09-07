using UnityEngine;
using NaughtyAttributes;
public class CameraController : MonoBehaviour
{
    [BoxGroup("Parameters"), SerializeField] private GameObject _target;
    [BoxGroup("Parameters"), SerializeField] private Vector3 _offset;
    [BoxGroup("Parameters"), SerializeField] private float _durability; //from 0 to 1
    [BoxGroup("Parameters"), SerializeField] private float _staticYPosition;
    [Space]
    [BoxGroup("Parameters"), SerializeField] private Vector3 m_defaultPosition;
    [BoxGroup("Parameters"), SerializeField] private Vector3 m_defaultRotation;
    [Space]
    [BoxGroup("Spec parameters"), SerializeField] private bool m_isConnectWithFace; // for First Person View

    [BoxGroup("Spec parameters"), SerializeField, ShowIf("m_isConnectWithFace")] private Transform m_characterHead;

    [SerializeField] private CameraController m_secondCamera;

    [HideInInspector] public bool m_rotateAroundCharacter;
    [HideInInspector]
    public Vector3 m_HeightDifference()
    {
        Vector3 heightDifference = transform.position - m_startingPosition;
        return heightDifference;
    }

    private bool m_enableFreeCamera;
    private bool m_canReturnCameraToBase;

    private float m_interpVelocity;
    private float m_interpVelocityForce = 20f;
    private float m_returnToBaseSpeed = 0.1f;
    public float smoothTime = 0.3F;

    private Vector3 m_targetPos;
    private Vector3 m_startingPosition;

    private Transform m_winnerZoneTransform;

    private float m_headStaticY;

    void Awake()
    {
        m_winnerZoneTransform = GameObject.FindGameObjectWithTag("WinnerZone").transform;
        m_targetPos = transform.position;

        if (m_isConnectWithFace)
        {
            m_headStaticY = m_characterHead.position.y;
        }

        m_startingPosition = transform.position;
    }

    private void Start()
    {
        StartingSetup();
    }

    void FixedUpdate()
    {
        if (m_rotateAroundCharacter)
        {
            transform.LookAt(_target.transform);
            transform.Translate(Vector3.right * 4f * Time.deltaTime);
        }
    }

    private void FollowForPlayer()
    {
        if (m_rotateAroundCharacter)
        {
            transform.LookAt(m_winnerZoneTransform);
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        else
        {
            Vector3 newPosition = Vector3.zero;

            if (m_isConnectWithFace)
            {
                if (m_enableFreeCamera)
                {
                    newPosition = new Vector3(0f, m_characterHead.position.y, m_characterHead.position.z) + _offset;
                }
                else
                {
                    newPosition = new Vector3(0f, _staticYPosition, m_characterHead.position.z) + _offset;
                }
            }
            else
            {
                Vector3 nativePosition = transform.position;
                Vector3 targetDirection = (_target.transform.position - nativePosition);
                m_interpVelocity = targetDirection.magnitude * m_interpVelocityForce;
                m_targetPos = transform.position + (targetDirection.normalized * m_interpVelocity * Time.fixedDeltaTime);

                if (m_enableFreeCamera)
                {
                    //newPosition = new Vector3(m_targetPos.x + _offset.x, _staticYPosition, m_targetPos.z + _offset.z);
                    newPosition = m_targetPos + new Vector3(_offset.x, 1.65f + _offset.y, _offset.z);
                }
                else
                {
                    newPosition = new Vector3(m_targetPos.x + _offset.x, _staticYPosition, m_targetPos.z + _offset.z);
                }
            }

            transform.position = Vector3.Lerp(transform.position, newPosition, _durability);
        }
    }


    public void ChangeActiveCamera()
    {
        m_secondCamera.gameObject.SetActive(true);
        m_secondCamera.transform.position = transform.position;
        m_secondCamera.m_rotateAroundCharacter = true;
        gameObject.SetActive(false);
    }

    public void EnableFreeCamera(bool state)
    {
        m_enableFreeCamera = state;
    }

    public void FreeFromParent()
    {
        transform.parent = null;
    }


    public void ResetPosition()
    {
        transform.position = m_winnerZoneTransform.position;
    }

    private void StartingSetup()
    {
        transform.localPosition = m_defaultPosition;
        transform.localRotation = Quaternion.Euler(m_defaultRotation);
    }

    public void AllowToReturnCamera(bool state)
    {
        m_canReturnCameraToBase = state;
    }
}
