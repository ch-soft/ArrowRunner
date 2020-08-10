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

[RequireComponent(typeof(Rigidbody))]
public abstract class GrabbingBaseObject : MonoBehaviour
{
    [BoxGroup("Settings"), SerializeField] private float m_pullingForce;
    [BoxGroup("Settings")] public GrabbingObjectType m_grabbingObjectType;

    [HideInInspector] public Vector3 m_pullingDirection;

    private Rigidbody m_rigidbody;

    private bool m_isgrabbing;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (m_rigidbody.velocity.magnitude > 0.1f)
        {
            //Vector3 newDirection = Vector3.RotateTowards(transform.forward, m_pullingDirection, m_pullingForce * Time.deltaTime, 20.0f);

            //transform.rotation = Quaternion.LookRotation(newDirection);

            if (m_isgrabbing)
            {
                transform.RotateAround(transform.position, Vector3.left, m_pullingForce * 2f * Time.deltaTime);
            }
        }
    }

    public IEnumerator PullObjectToPlayer()
    {
        m_isgrabbing = true;
        m_rigidbody.AddForce(m_pullingDirection + new Vector3(2f * transform.position.x * -m_pullingForce, 1.5f * -m_pullingForce, 3f * -m_pullingForce));
        yield return new WaitForSeconds(2f);
        m_isgrabbing = false;
    }

    public void PrepareGrabbingObject(Vector3 playerDirection)
    {
        m_pullingDirection = playerDirection;
    }
}

public interface IOnHookGrab
{
    void OnHookGrab();
}
