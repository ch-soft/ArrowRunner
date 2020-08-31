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

    private float m_maxDistToPlayer = 17f;
    private float m_prepareWeaponDistance = 15f;
    private float m_hitPlayerDistance = 7f;

    private bool m_weaponIsReady;
    private bool m_playerWasHit;

    private void Update()
    {
        if (m_parentEnemy.m_isAlive)
        {
            if (m_playerIsInSight)
            {
                ChangeColorDueDistance();
            }
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

        //print(dist);

        //We will check distance to player and start kill player here
        for (int i = 0; i < m_selfRenderer.materials.Length; i++)
        {
            m_selfRenderer.materials[i].color = Color.Lerp(m_closeColor, m_farColor, dist / m_maxDistToPlayer);
        }

        if (dist <= m_prepareWeaponDistance)
        {
            if (!m_weaponIsReady)
            {
                m_parentEnemy.PlayPrepareWeaponAnim();
                m_weaponIsReady = true;
            }

            if (dist <= m_hitPlayerDistance)
            {
                if (!m_playerWasHit)
                {
                    m_parentEnemy.PlaySwordSlashAnim();
                    m_playerWasHit = true;
                }
            }
        }
    }
}
