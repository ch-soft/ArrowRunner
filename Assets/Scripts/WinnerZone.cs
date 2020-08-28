using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class WinnerZone : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] private LevelController m_levelManager;

    private bool m_isLevelFinished;
    private PlayerInstance m_playerInstance;

    private void Awake()
    {
        m_playerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInstance>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerInstance>())
        {
            m_levelManager.OpenWinnerPanel();
            TimeControl.m_levelFinished = true;
            m_playerInstance.AllowToRun(false);
            StartCoroutine(m_playerInstance.PlayDancingAnimation());
        }
    }
}
