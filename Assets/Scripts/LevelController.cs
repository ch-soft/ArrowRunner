using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [BoxGroup("Buttons"), SerializeField] private Button m_levelMenuButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_restartButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_continueButton;

    [BoxGroup("References"), SerializeField] private Animator m_winnerAnimator;
    [BoxGroup("References"), SerializeField] private Animator m_defeatAnimator;

    private void Awake()
    {
        m_levelMenuButton.onClick.AddListener(OpenDefeatPanel);
        m_restartButton.onClick.AddListener(RestartLevel);
        m_continueButton.onClick.AddListener(RestartLevel);
    }

    private void Update()
    {
        TimeControl.AutoNormalizeTime();
    }

    private void Start()
    {
        StartCoroutine(TimeControl.NormalizeTime(0f));

        TimeControl.m_levelFinished = false;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenDefeatPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            m_defeatAnimator.Play("ShowEndgamePanel");
        }
    }

    public void OpenWinnerPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            m_winnerAnimator.Play("ShowEndgamePanel");
        }
    }
}
