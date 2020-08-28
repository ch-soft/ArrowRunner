using System.Collections;
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
    [HideInInspector] public bool m_objectWasAttracted;

    private float m_grabbingTime = 1f;
    private bool m_isGrabbing;

    //Bridge
    private Vector3 m_targetRotation = new Vector3(-90f, 0f, 0f);
    private float m_fallingBridgeForce;

    //Grappling base

    [HideInInspector] public PlayerInstance m_playerInstance;
    private Transform m_playerInstanceTransform;



    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_playerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInstance>();
        m_playerInstanceTransform = m_playerInstance.transform;
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

                        transform.position = Vector3.MoveTowards(transform.position, m_pullingObject.position, Time.deltaTime * m_pullingForce / 8f);
                        if (Vector3.Distance(transform.position, m_pullingDirection) < 5f)
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
                        //if (Vector3.Distance(transform.position, m_playerInstanceTransform.position) > 4f)
                        //{
                        //    m_playerInstance.transform.position = Vector3.MoveTowards(m_playerInstance.transform.position, transform.position, Time.deltaTime * 14f);
                        //}
                        //else
                        //{
                        //    m_isGrabbing = false;
                        //    m_playerInstance.EnableFreeJump(false);
                        //}

                        break;
                    }
            }
        }
    }

    public IEnumerator PullObjectToPlayer()
    {
        m_objectWasAttracted = true;
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
        m_objectWasAttracted = true;
    }

    public IEnumerator MakeGrapplingMove(float timeInAir, float animSpeed)
    {
        m_isGrabbing = true;
        m_objectWasAttracted = true;
        m_playerInstance.EnableRigidbodyJump(animSpeed);
        //m_playerInstance.
        //m_playerInstance.EnableFreeJump(true);


        yield return new WaitForSecondsRealtime(timeInAir); //можно сделать зависимость этого числа от расстояния до персонажа
        m_playerInstance.DisableRigidbodyJump(timeInAir * 2.0f);
        //m_playerInstance.EnableFreeJump(false);

        m_isGrabbing = false;
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
