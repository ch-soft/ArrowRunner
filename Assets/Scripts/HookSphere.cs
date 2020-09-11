using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSphere : MonoBehaviour
{

    private GrabbingEnemy m_currentEnemy;
    private GrabbingBridge m_currentBridge;
    private GrapplingBase m_currentGrapplingBase;
    private GrabbingBarrel m_currentBarrel;
    private GrapplingElevatorUp m_currentElevatorUp;

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
                    m_currentBridge.SwitchOutlineState(true);
                    break;
                }
            case "GrapplingSurface":
                {
                    m_currentGrapplingBase = other.gameObject.GetComponent<GrapplingBase>();
                    m_currentGrapplingBase.SwitchOutlineState(true);
                    break;
                }
            case "Barrel":
                {
                    m_currentBarrel= other.gameObject.GetComponent<GrabbingBarrel>();
                    m_currentBarrel.SwitchOutlineState(true);
                    break;
                }
            case "ElevatorUp":
                {
                    m_currentElevatorUp = other.gameObject.GetComponent<GrapplingElevatorUp>();
                    m_currentElevatorUp.SwitchOutlineState(true);
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
                    m_currentBridge.SwitchOutlineState(false);
                    break;
                }
            case "GrapplingSurface":
                {
                    m_currentGrapplingBase = other.gameObject.GetComponent<GrapplingBase>();
                    m_currentGrapplingBase.SwitchOutlineState(false);
                    break;
                }
            case "Barrel":
                {
                    m_currentBarrel = other.gameObject.GetComponent<GrabbingBarrel>();
                    m_currentBarrel.SwitchOutlineState(false);
                    break;
                }
            case "ElevatorUp":
                {
                    m_currentElevatorUp = other.gameObject.GetComponent<GrapplingElevatorUp>();
                    m_currentElevatorUp.SwitchOutlineState(false);
                    break;
                }
        }
    }

    private void OnDisable()
    {
        if (m_currentEnemy != null)
        {
            m_currentEnemy.SwitchOutlineWtate(false);
        }

        if (m_currentBridge != null)
        {
            m_currentBridge.SwitchOutlineState(false);
        }

        if (m_currentGrapplingBase != null)
        {
            m_currentGrapplingBase.SwitchOutlineState(false);
        }
        if (m_currentBarrel != null)
        {
            m_currentBarrel.SwitchOutlineState(false);
        }
        if (m_currentElevatorUp != null)
        {
            m_currentElevatorUp.SwitchOutlineState(false);
        }
    }
}
