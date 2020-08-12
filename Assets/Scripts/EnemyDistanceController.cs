using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class EnemyDistanceController : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] private GrabbingEnemy m_parentEnemy;
    [BoxGroup("References"), SerializeField] private Renderer m_selfRenderer;
    [Space]
    [BoxGroup("Settings"), SerializeField] private Color m_farColor;
    [BoxGroup("Settings"), SerializeField] private Color m_closeColor;

    private Transform m_playerTransform;

    private bool m_playerIsInSight;

    private float m_maxDistToPlayer = 5f;

    private void Update()
    {
        if (m_playerIsInSight)
        {
            ChangeColorDueDistance();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                {
                    m_playerTransform = other.transform;
                    ChangePlayerIsInSight(true);
                    break;
                }
        }
    }



    private void ChangePlayerIsInSight(bool state)
    {
        m_playerIsInSight = state;
    }

    private void ChangeColorDueDistance()
    {
        float dist = Vector3.Distance(m_playerTransform.position, transform.position);
        //We will check distance to player and start kill player here
        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
        {
            m_selfRenderer.materials[i].color = Color.Lerp(m_closeColor, m_farColor, dist / 15f);
        }
    }

    private void LookAtPlayer()
    {
        transform.LookAt(m_playerTransform);
    }
}
