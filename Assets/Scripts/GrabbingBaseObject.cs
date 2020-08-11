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
    PullUpFastening
}

public abstract class GrabbingBaseObject : MonoBehaviour
{
    [BoxGroup("Settings"), SerializeField] private float m_pullingForce;
    [BoxGroup("Settings"), SerializeField] private Vector3 m_forceDirection;
    [BoxGroup("Settings")] public GrabbingObjectType m_grabbingObjectType;
    [Space]
    [BoxGroup("References"), SerializeField] private Rigidbody m_rigidbody;

    [HideInInspector] public Vector3 m_pullingDirection;
    [HideInInspector] public Transform m_pullingObject;


    private float m_grabbingTime = 1f;

    private bool m_isgrabbing;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (m_grabbingObjectType)
        {
            case GrabbingObjectType.EnemyCharacter:
                {
                    //if (m_rigidbody.velocity.magnitude > 0.1f)
                    //{
                    //    //Vector3 newDirection = Vector3.RotateTowards(transform.forward, m_pullingDirection, m_pullingForce * Time.deltaTime, 20.0f);

                    //    //transform.rotation = Quaternion.LookRotation(newDirection);

                    if (m_isgrabbing)
                    {
                        transform.RotateAround(transform.position, Vector3.left, m_pullingForce * 10f * Time.deltaTime);

                        transform.position = Vector3.MoveTowards(transform.position, m_pullingObject.position, Time.deltaTime * m_pullingForce / 4f);
                        if (Vector3.Distance(transform.position, m_pullingDirection) < 3f)
                        {
                            m_isgrabbing = false;
                        }
                    }
                    //}
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
