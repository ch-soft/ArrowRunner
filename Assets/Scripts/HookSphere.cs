using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSphere : MonoBehaviour
{

    private GrabbingEnemy m_currentEnemy;
    private GrabbingBridge m_currentBridge;


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                {
                    m_currentEnemy = other.gameObject.GetComponent<GrabbingEnemy>();
                    m_currentEnemy.SwitchOutlineWtate(true);
                    break;
                }
            case "Bridge":
                {
                    m_currentBridge = other.gameObject.GetComponent<GrabbingBridge>();
                    m_currentBridge.SwitchOutlineWtate(true);
                    break;
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                {
                    m_currentEnemy = other.gameObject.GetComponent<GrabbingEnemy>();
                    m_currentEnemy.SwitchOutlineWtate(false);
                    break;
                }
            case "Bridge":
                {
                    m_currentBridge = other.gameObject.GetComponent<GrabbingBridge>();
                    m_currentBridge.SwitchOutlineWtate(false);
                    break;
                }
        }
    }

    private void OnDisable()
    {
        if(m_currentEnemy != null)
        {
            m_currentEnemy.SwitchOutlineWtate(false);
        }

        if (m_currentBridge != null)
        {
            m_currentBridge.SwitchOutlineWtate(false);
        }
    }
}
