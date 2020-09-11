using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class CheatPanel : MonoBehaviour
{
    [SerializeField] private Button m_closePanelButton;
    [Space]
    [SerializeField] private Button m_openNextLevelButton;
    [SerializeField] private Button m_openPrevLevelButton;
    [SerializeField] private Button m_nullifyLevelsButton;


    private LevelController m_levelManager;
    private Animator m_animator;

    private float _cheatPanelTimer;


    private void Awake()
    {
        m_levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelController>();

        m_animator = GetComponent<Animator>();

        m_closePanelButton.onClick.AddListener(ClosePanel);

        m_openNextLevelButton.onClick.AddListener(m_levelManager.OpenNextLevel);
        m_openPrevLevelButton.onClick.AddListener(m_levelManager.OpenPrevLevel);
        m_nullifyLevelsButton.onClick.AddListener(m_levelManager.FullResetLevelsInfo);
    }

    private void OpenPanel()
    {
        m_animator.Play("OpenCheatPanel");
        StartCoroutine(TimeControl.NormalizeTime(0f));
    }

    private void ClosePanel()
    {
        m_animator.Play("CloseCheatPanel");

    }

    private void Update()
    {
        CheatPanelProcessing();
    }

    private void CheatPanelProcessing()
    {
        if ((Input.touchCount == 3) || (Input.GetMouseButton(2))) _cheatPanelTimer += Time.unscaledTime;
        else _cheatPanelTimer = 0f;

        if (_cheatPanelTimer > 1f)
        {
            OpenPanel();
            _cheatPanelTimer = 0f;
        }
    }
}
