using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class CameraController : MonoBehaviour
{
    [BoxGroup("Parameters"), SerializeField] private GameObject _target;
    [BoxGroup("Parameters"), SerializeField] private Vector3 _offset;
    [BoxGroup("Parameters"), SerializeField] private float _durability; //from 0 to 1
    [BoxGroup("Parameters"), SerializeField] private float _staticYPosition;
    [Space]
    [BoxGroup("Spec parameters"), SerializeField] private bool m_isConnectWithFace; // for First Person View

    [BoxGroup("Spec parameters"), SerializeField, ShowIf("m_isConnectWithFace")] private Transform m_characterHead;

    private bool m_enableFreeCamera;

    private float m_interpVelocity;
    private float m_interpVelocityForce = 20f;

    private Vector3 m_targetPos;
    private float m_headStaticY;

    void Start()
    {
        m_targetPos = transform.position;

        if (m_isConnectWithFace)
        {
            m_headStaticY = m_characterHead.position.y;
        }
    }

    void FixedUpdate()
    {
        FollowForPlayer();
    }

    private void FollowForPlayer()
    {
        if (m_isConnectWithFace)
        {
            if (m_enableFreeCamera)
            {
                transform.position = new Vector3(0f, m_characterHead.position.y + 0.5f, m_characterHead.position.z) + _offset;
            }
            else
            {
                transform.position = new Vector3(0f, _staticYPosition, m_characterHead.position.z) + _offset;
            }

        }
        else
        {
            Vector3 nativePosition = transform.position;
            Vector3 targetDirection = (_target.transform.position - nativePosition);
            m_interpVelocity = targetDirection.magnitude * m_interpVelocityForce;
            m_targetPos = transform.position + (targetDirection.normalized * m_interpVelocity * Time.fixedDeltaTime);

            Vector3 newPosition;
            if (m_enableFreeCamera)
            {
                newPosition = m_targetPos + new Vector3(_offset.x, 0.5f, _offset.z);
            }
            else
            {
                newPosition = new Vector3(m_targetPos.x + _offset.x, _staticYPosition, m_targetPos.z + _offset.z);

            }
            transform.position = Vector3.Lerp(transform.position, newPosition, _durability);
        }
    }

    public void EnableFreeCamera(bool state)
    {
        m_enableFreeCamera = state;
    }
}
