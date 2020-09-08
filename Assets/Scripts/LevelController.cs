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
    [BoxGroup("Buttons"), SerializeField] private Button m_tapToPlayButton;

    [BoxGroup("References"), SerializeField] private Animator m_winnerAnimator;
    [BoxGroup("References"), SerializeField] private Animator m_defeatAnimator;
    private PlayerInstance m_playerInstance;

    private const int m_currentLevel = 1;

    private void Awake()
    {
        AnalyticsManager.Initialize();

        m_playerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInstance>();

        m_tapToPlayButton.onClick.AddListener(StartRun);

        m_levelMenuButton.onClick.AddListener(OpenDefeatPanel);
        m_restartButton.onClick.AddListener(RestartLevel);
        m_continueButton.onClick.AddListener(RestartLevel);
    }

    //private void Update()
    //{
    //    TimeControl.AutoNormalizeTime();
    //}

    private void Start()
    {
        StartCoroutine(TimeControl.NormalizeTime(0f));

        TimeControl.m_levelFinished = false;
    }

    private void StartRun()
    {
        m_playerInstance.AllowToRun(true);

        m_playerInstance.PlayRunAnimation();

        m_tapToPlayButton.gameObject.SetActive(false);

        AnalyticsManager.FireRoundStartEvent(m_currentLevel, 0);
    }

    private void RestartLevel()
    {
        TimeControl.NormalizeTime(0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenDefeatPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            AnalyticsManager.FireRoundFailEvent(m_currentLevel, 0, 0, 100);//Need consider 2, 3, 4

            AnalyticsManager.FireRoundEndEvent(m_currentLevel, 0, 0);

            m_defeatAnimator.Play("ShowEndgamePanel");
        }
    }

    public void OpenWinnerPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            AnalyticsManager.FireRoundCompleteEvent(m_currentLevel, 0, 0); //Need consider 2nd & 3rd 

            AnalyticsManager.FireRoundEndEvent(m_currentLevel, 0, 0);

            m_winnerAnimator.Play("ShowEndgamePanel");
        }
    }
}
