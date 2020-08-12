using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum GrabbingObjectType
{
    None = 0,
    GrabObject,
    EnemyCharacter,
    EnvironmentObject,
    Bridge
}

public abstract class GrabbingBaseObject : MonoBehaviour
{
    [BoxGroup("Settings"), SerializeField] private float m_pullingForce;
    [BoxGroup("Settings"), SerializeField] private Vector3 m_forceDirection;
    [BoxGroup("Settings")] public GrabbingObjectType m_grabbingObjectType;
    [Space]
    [BoxGroup("References"), SerializeField] public Rigidbody m_rigidbody;

    [HideInInspector] public Vector3 m_pullingDirection;
    [HideInInspector] public Transform m_pullingObject;


    private float m_grabbingTime = 1f;

    private Vector3 m_targetRotation = new Vector3(-90f, 0f, 0f);

    private bool m_isgrabbing;

    private float m_fallingBridgeForce;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        switch (m_grabbingObjectType)
        {
            case GrabbingObjectType.EnemyCharacter:
                {
                    if (m_isgrabbing)
                    {
                        transform.RotateAround(transform.position, Vector3.left, m_pullingForce * 10f * Time.fixedDeltaTime);

                        transform.position = Vector3.MoveTowards(transform.position, m_pullingObject.position, Time.deltaTime * m_pullingForce / 4f);
                        if (Vector3.Distance(transform.position, m_pullingDirection) < 3f)
                        {
                            m_isgrabbing = false;
                        }
                    }
                    break;
                }
            case GrabbingObjectType.Bridge:
                {
                    if (m_isgrabbing)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(m_targetRotation), Time.fixedDeltaTime * m_fallingBridgeForce);
                        m_fallingBridgeForce += 1f;
                    }
                    break;
                }
        }
    }

    public IEnumerator PullObjectToPlayer()
    {
        m_rigidbody.useGravity = true;
        m_isgrabbing = true;
        m_rigidbody.AddForce(m_pullingDirection + new Vector3(m_forceDirection.x * transform.position.x * -m_pullingForce, m_forceDirection.y * -m_pullingForce, m_forceDirection.z * -m_pullingForce));
        yield return new WaitForSeconds(m_grabbingTime);
        m_isgrabbing = false;
    }

    public void PullBridge()
    {
        m_isgrabbing = true;
        m_fallingBridgeForce = m_pullingForce;
    }

    public void PrepareGrabbingObject(Vector3 playerDirection, Transform hook)
    {
        m_pullingDirection = playerDirection;
        m_pullingObject = hook;
    }
}

public interface IOnHookGrab
{
    void OnHookGrab();
}
