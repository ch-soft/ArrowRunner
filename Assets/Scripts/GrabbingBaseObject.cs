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
    Bridge,
    GrapplingBase
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
    private bool m_isGrabbing;

    //Bridge
    private Vector3 m_targetRotation = new Vector3(-90f, 0f, 0f);
    private float m_fallingBridgeForce;

    //Grappling base

    private Transform m_characterTransform;
    private Rigidbody m_characterRigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (m_isGrabbing)
        {
            switch (m_grabbingObjectType)
            {
                case GrabbingObjectType.EnemyCharacter:
                    {

                        transform.RotateAround(transform.position, Vector3.left, m_pullingForce * 10f * Time.fixedDeltaTime);

                        transform.position = Vector3.MoveTowards(transform.position, m_pullingObject.position, Time.deltaTime * m_pullingForce / 4f);
                        if (Vector3.Distance(transform.position, m_pullingDirection) < 3f)
                        {
                            m_isGrabbing = false;
                        }
                        break;
                    }
                case GrabbingObjectType.Bridge:
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(m_targetRotation), Time.fixedDeltaTime * m_fallingBridgeForce);
                        m_fallingBridgeForce += 1f;
                        break;
                    }

                case GrabbingObjectType.GrapplingBase:
                    {
                        Vector3 grapplePoint = m_characterTransform.position;
                        Vector3 grappleDirection = (grapplePoint - transform.position);

                        float distance = Vector3.Distance(grapplePoint, transform.position);

                        //if (distance < grappleDirection.magnitude)// With this you can determine if you are overshooting your target. You are basically checking, if you are further away from your target then during the last frame
                        //{
                            float velocity = m_characterRigidbody.velocity.magnitude;//How fast you are currently

                            Vector3 newDirection = Vector3.ProjectOnPlane(m_characterRigidbody.velocity, grappleDirection);//So this is a bit more complicated
                                                                                                                           //basically I am using the grappleDirection Vector as a normal vector of a plane.
                                                                                                                           //I am really bad at explaining it. Partly due to my bad english but it is best if you just look up what Vector3.ProjectOnPlane does.

                            m_characterRigidbody.velocity = newDirection.normalized * velocity;//Now I just have to redirect the velocity

                        //}
                        break;
                    }
            }
        }
    }

    public IEnumerator PullObjectToPlayer()
    {
        m_rigidbody.useGravity = true;
        m_isGrabbing = true;
        m_rigidbody.AddForce(m_pullingDirection + new Vector3(m_forceDirection.x * transform.position.x * -m_pullingForce, m_forceDirection.y * -m_pullingForce, m_forceDirection.z * -m_pullingForce));
        yield return new WaitForSeconds(m_grabbingTime);
        m_isGrabbing = false;
    }

    public void PullBridge()
    {
        m_isGrabbing = true;
        m_fallingBridgeForce = m_pullingForce;
    }

    public void PrepareGrapplingMove()
    {
        m_isGrabbing = true;
    }

    public void PrepareGrabbingObject(Vector3 playerDirection, Transform hook, Transform characterTransform, Rigidbody characterRigidbody)
    {
        m_pullingDirection = playerDirection;
        m_pullingObject = hook;

        m_characterTransform = characterTransform;
        m_characterRigidbody = characterRigidbody;
    }
}

public interface IOnHookGrab
{
    void OnHookGrab();
}
